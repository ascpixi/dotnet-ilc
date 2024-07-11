using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using ILC;

var dotnetVer = RuntimeInformation.FrameworkDescription[5..];

var selfDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
var pkgDir = Path.Combine(selfDir, "pkg", dotnetVer);
var ilcPath = Path.Combine(pkgDir, "tools", OperatingSystem.IsWindows() ? "ilc.exe" : "ilc");

if (File.Exists(ilcPath))
    return InvocationForwarder.Run(ilcPath, args);

var platform =
    RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux"
    : RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win"
    : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx"
    : null;

if (platform == null) {
    Console.Error.WriteLine($"wrapper error: unknown platform ({RuntimeInformation.RuntimeIdentifier}) - please report this to https://github.com/ascpixi/dotnet-ilc");
    return 1;
}

var rid = $"{platform}-{RuntimeInformation.OSArchitecture.ToString().ToLower()}";

Console.WriteLine($"wrapper: retrieving ilc for .NET {dotnetVer} via NuGet...");

var pkgName = $"runtime.{rid}.microsoft.dotnet.ilcompiler";

using var http = new HttpClient();
var res = await http.GetAsync($"https://api.nuget.org/v3-flatcontainer/{pkgName}/{dotnetVer}/{pkgName}.{dotnetVer}.nupkg");
if (!res.IsSuccessStatusCode) {
    Console.Error.WriteLine($"wrapper error: retrieval failed! HTTP {(int)res.StatusCode} ({res.StatusCode}) for package '{pkgName}' with version '{dotnetVer}'");
    return 1;
}

Directory.CreateDirectory(Path.GetDirectoryName(pkgDir)!);

var pkg = new ZipArchive(await res.Content.ReadAsStreamAsync(), ZipArchiveMode.Read);
pkg.ExtractToDirectory(pkgDir, overwriteFiles: true);

if (!File.Exists(ilcPath)) {
    Console.Error.WriteLine("wrapper error: the package was retrieved, but the ilc binary is missing");
    Console.Error.WriteLine($"wrapper error: package contents can be found in '{pkgDir}'");
    return 1;
}

if (OperatingSystem.IsLinux()) {
    File.SetUnixFileMode(ilcPath, File.GetUnixFileMode(ilcPath) | UnixFileMode.UserExecute);
}

return InvocationForwarder.Run(ilcPath, args);