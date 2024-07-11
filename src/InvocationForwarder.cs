using System.Diagnostics;

namespace ILC;

/// <summary>
/// Forwards command-line invocations.
/// </summary>
public class InvocationForwarder
{
    public static int Run(string target, string[] args)
    {
        var psi = new ProcessStartInfo() {
            WorkingDirectory = Directory.GetCurrentDirectory(),
            FileName = target
        };

        foreach (var arg in args) {
            psi.ArgumentList.Add(arg);
        }

        var process = Process.Start(psi)!;
        process.WaitForExit();
        return process.ExitCode;
    }
}