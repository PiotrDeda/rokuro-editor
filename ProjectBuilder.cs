using System;
using System.Diagnostics;

namespace RokuroEditor;

public static class ProjectBuilder
{
	public static void Build(string projectPath, string projectName)
	{
		using (var process = new Process())
		{
			process.StartInfo = new() {
				FileName = "cmd.exe",
				Arguments = $"/C dotnet build \"{projectPath}\" --output build/{projectName}",
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true
			};
			process.Start();
			Console.Write(process.StandardOutput.ReadToEnd());
			process.WaitForExit();
		}
	}

	public static Process Run(Process process, string projectName)
	{
		process.StartInfo.FileName = $"build/{projectName}/{projectName}.exe";
		process.StartInfo.WorkingDirectory = $"build/{projectName}";
		process.Start();
		return process;
	}
}
