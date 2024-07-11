# dotnet-ilc
`dotnet-ilc` is a simple .NET tool wrapper over the `ilc` compiler provided by .NET 7.0 and higher, which is used for Native AOT (NAOT) compilation.

This tool is available under the NuGet package name `ilc`. In order to install it, use the following command:
```
dotnet tool install -g ilc
```

The tool will automatically retrieve the right versions based on the .NET runtime that is used to invoke the tool. This means that you might experience a measurable delay the first time you launch the tool with a new .NET version.

## Use-cases
This tool is mainly intended for use when compiling with a custom set of runtime libraries and/or assemblies. If you wish to compile with .NET's runtime libraries, it's recommended to use a regular .NET project. Alternatively, you can reference the assemblies from the coressponding `runtime.<rid>.microsoft.dotnet.ilcompiler` packages, where `<rid>` is the runtime identifier of the **target** operating system.

If you're using C#, you can also check out [bflat](https://flattened.net/), which also uses `ilc` under the hood, but automatically compiles C# files to IL assemblies.

## Why a wrapper?
The internal NuGet packages used to store `ilc` - `runtime.<rid>.microsoft.dotnet.ilcompiler` - are meant to be used as project dependencies. These packages also supply various build specification files, which reference the stored `ilc` binary.

This wrapper removes the need for scripts to manually locate the NuGet global packages folder, or to use hacks such as creating a temporary project just to install the internal package. It automatically determines the host system runtime ID and .NET version, and retrieves the right package. In case of an non-major .NET version upgrade, the tool will automatically retrieve new files.
