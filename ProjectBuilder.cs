using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace RokuroEditor;

public static class ProjectBuilder
{
	public static void CreateProject(string projectPath, string projectName, string dotNetPath, Action<string> log)
	{
		using (var process = new Process())
		{
			process.StartInfo = new() {
				FileName = "cmd.exe",
				Arguments = $"/C {dotNetPath} new sln --name \"{projectName}\" --output \"{projectPath}\" && " +
							$"{dotNetPath} new console --name \"{projectName}\" --output \"{projectPath}\" && " +
							$"cd {projectPath} && " +
							$"{dotNetPath} sln add \"{projectPath}/{projectName}.csproj\"",
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				StandardOutputEncoding = Encoding.UTF8
			};
			process.Start();
			log(process.StandardOutput.ReadToEnd());
			process.WaitForExit();
		}

		File.Copy("assets_editor/templates/Program.cstemplate", $"{projectPath}/Program.cs", true);
		string sanitizedProjectName = projectName;
		Regex.Replace(sanitizedProjectName, @"\s+", "");
		File.WriteAllLines($"{projectPath}/Program.cs", File.ReadAllLines($"{projectPath}/Program.cs")
			.Select(line => line.Replace("%{ProjectName}%", sanitizedProjectName))
		);

		// TODO: Remove when nuget package is published
		File.Copy("assets_editor/templates/csproj.cstemplate", $"{projectPath}/{projectName}.csproj", true);
	}

	public static void Build(string projectPath, string projectName, string dotNetPath, Action<string> log)
	{
		using (var process = new Process())
		{
			process.StartInfo = new() {
				FileName = "cmd.exe",
				Arguments =
					$"/C {dotNetPath} build \"{projectPath}/{projectName}.csproj\" --output build/{projectName}",
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				StandardOutputEncoding = Encoding.UTF8
			};
			process.Start();
			log(process.StandardOutput.ReadToEnd());
			process.WaitForExit();
		}
	}

	public static Process Run(Process process, string projectName, Action<string> log)
	{
		process.StartInfo.FileName = $"build/{projectName}/{projectName}.exe";
		process.StartInfo.WorkingDirectory = $"build/{projectName}";
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
		process.OutputDataReceived += (_, args) => {
			if (!string.IsNullOrEmpty(args.Data))
				log(args.Data);
		};
		process.Start();
		return process;
	}

	public static List<string> GetScenePaths(string projectName) =>
		Directory.Exists($"build/{projectName}/assets/autogen/data/scenes")
			? Directory.GetFiles($"build/{projectName}/assets/autogen/data/scenes", "*.json",
				SearchOption.AllDirectories).ToList()
			: new();
}
