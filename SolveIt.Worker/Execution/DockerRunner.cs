using System.Diagnostics;

namespace SolveIt.Worker.Execution;

public static class DockerRunner
{
    public static async Task<ExecutionResult> ExecuteAsync(
        string language,
        string sourceCode,
        string input,
        int timeLimitMs,
        CancellationToken ct)
    {
        var tempDir = Path.Combine(
            Path.GetTempPath(),
            "solveit",
            Guid.NewGuid().ToString());

        Directory.CreateDirectory(tempDir);

        try
        {
            var sourceFile = GetSourceFile(language, tempDir);
            await File.WriteAllTextAsync(sourceFile, sourceCode, ct);

            var inputFile = Path.Combine(tempDir, "input.txt");
            await File.WriteAllTextAsync(inputFile, input, ct);

            var dockerArgs = BuildDockerCommand(language, tempDir);

            var psi = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = dockerArgs,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            var sw = Stopwatch.StartNew();

            using var process = Process.Start(psi);
            if (process == null)
                throw new Exception("Failed to start docker");

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            var finished = await Task.WhenAny(
                process.WaitForExitAsync(ct),
                Task.Delay(timeLimitMs, ct));

            sw.Stop();

            if (finished != process.WaitForExitAsync(ct))
            {
                try { process.Kill(true); } catch { }

                return ExecutionResult.CreateTimeout();
            }

            var output = (await outputTask).Trim();
            var error = (await errorTask).Trim();

            if (!string.IsNullOrWhiteSpace(error))
                return ExecutionResult.CreateRuntimeError(error);

            return ExecutionResult.CreateSuccess(output, sw.ElapsedMilliseconds);
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    private static string GetSourceFile(string language, string dir)
    {
        return language switch
        {
            "python" => Path.Combine(dir, "main.py"),
            "csharp" => Path.Combine(dir, "Program.cs"),
            "javascript" => Path.Combine(dir, "main.js"),
            "go" => Path.Combine(dir, "main.go"),
            "dart" => Path.Combine(dir, "main.dart"),
            _ => throw new NotSupportedException("Language not supported")
        };
    }

    private static string BuildDockerCommand(string language, string dir)
    {
        return language switch
        {
            "python" =>
                $"run --rm --network none --memory 256m --cpus 1 -v \"{dir}:/app\" python:3.11 bash -c \"python /app/main.py < /app/input.txt\"",

            "javascript" =>
                $"run --rm --network none --memory 256m --cpus 1 -v \"{dir}:/app\" node:20 bash -c \"node /app/main.js < /app/input.txt\"",

            "go" =>
                $"run --rm --network none --memory 256m --cpus 1 -v \"{dir}:/app\" golang:1.22 bash -c \"go run /app/main.go < /app/input.txt\"",

            "dart" =>
                $"run --rm --network none --memory 256m --cpus 1 -v \"{dir}:/app\" dart:stable bash -c \"dart /app/main.dart < /app/input.txt\"",

            "csharp" =>
                $"run --rm --network none --memory 256m --cpus 1 -v \"{dir}:/app\" mcr.microsoft.com/dotnet/sdk:8.0 bash -c \"dotnet new console -n app && mv /app/Program.cs app/Program.cs && cd app && dotnet run < /app/input.txt\"",

            _ => throw new NotSupportedException("Language not supported")
        };
    }
}
