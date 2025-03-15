using System.Diagnostics;

namespace BenchmarkingDapperEFCoreCRMSqlServer.Utils;

public static class CommandLineHelper
{
    public static void Execute(string command, string message)
    {
        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = "pwsh",
            Arguments = $"-c \"echo '# {message}'; echo ''; {command}; echo '';\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = new Process { StartInfo = processStartInfo })
        {
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Console.WriteLine(output);
            if (!string.IsNullOrWhiteSpace(error))
            {
                Console.WriteLine("Erro:");
                Console.WriteLine(error);
            }
        }
    }
}