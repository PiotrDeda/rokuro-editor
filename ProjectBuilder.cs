using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RokuroEditor;

public static class ProjectBuilder
{
	public static void Build(string projectPath, string projectName)
	{
		using (var process = new Process())
		{
			process.StartInfo = new() {
				FileName = "cmd.exe",
				Arguments = $"/C dotnet build \"{projectPath}/{projectName}.csproj\" --output build/{projectName}",
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

	public static List<string> GetScenePaths(string projectName) => Directory
		.GetFiles($"build/{projectName}/assets/autogen/data/scenes", "*.json", SearchOption.AllDirectories).ToList();
}
