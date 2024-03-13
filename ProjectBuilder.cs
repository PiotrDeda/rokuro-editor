using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Rokuro.Graphics;
using RokuroEditor.Models;
using Camera = Rokuro.Graphics.Camera;
using GameObject = Rokuro.Objects.GameObject;

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
							$"{dotNetPath} sln add \"{projectPath}/{projectName}.csproj\" &&" +
							$"{dotNetPath} add package Rokuro --source https://nuget.pkg.github.com/PiotrDeda/index.json &&" +
							$"{dotNetPath} add package Sayers.SDL2.Core --version 1.0.11 &&" +
							$"{dotNetPath} restore",
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				StandardOutputEncoding = Encoding.UTF8
			};
			log("= Creating project...\n");
			process.Start();
			log(process.StandardOutput.ReadToEnd());
			log("= Finished creating project\n");
			process.WaitForExit();
		}

		File.Copy("assets_editor/templates/Program.cstemplate", $"{projectPath}/Program.cs", true);
		string sanitizedProjectName = projectName;
		Regex.Replace(sanitizedProjectName, @"\s+", "");
		File.WriteAllLines($"{projectPath}/Program.cs", File.ReadAllLines($"{projectPath}/Program.cs")
			.Select(line => line.Replace("%{ProjectName}%", sanitizedProjectName))
		);
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
			log("= Building project...\n");
			process.Start();
			log(process.StandardOutput.ReadToEnd());
			log("= Finished building project\n");
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
		log("= Running project...\n");
		process.Start();
		return process;
	}

	public static ProjectTypes GetTypes(string projectName)
	{
		string projectAssemblyPath = $"build/{projectName}/{projectName}.dll";
		string rokuroAssemblyPath = $"build/{projectName}/Rokuro.dll";
		var resolver = new PathAssemblyResolver(new List<string> {
			projectAssemblyPath,
			rokuroAssemblyPath,
			typeof(object).Assembly.Location,
			typeof(object).Assembly.Location.Replace("System.Private.CoreLib.dll", "System.Runtime.dll")
		});
		using var mlc = new MetadataLoadContext(resolver, typeof(object).Assembly.GetName().ToString());
		Assembly projectAssembly = mlc.LoadFromAssemblyPath(projectAssemblyPath);
		Assembly rokuroAssembly = mlc.LoadFromAssemblyPath(rokuroAssemblyPath);

		Type gameObjectType = rokuroAssembly.GetType(typeof(GameObject).FullName!)!;
		Type cameraType = rokuroAssembly.GetType(typeof(Camera).FullName!)!;
		Type spriteType = rokuroAssembly.GetType(typeof(Sprite).FullName!)!;
		return new(new(rokuroAssembly.GetTypes().Concat(projectAssembly.GetTypes())
				.Where(type => gameObjectType.IsAssignableFrom(type) && !type.IsAbstract)
				.Select(type => GameObjectType.FromType(type)).ToList()),
			new(rokuroAssembly.GetTypes().Concat(projectAssembly.GetTypes())
				.Where(type => cameraType.IsAssignableFrom(type) && !type.IsAbstract)
				.Select(type => CameraType.FromType(type)).ToList()),
			new(rokuroAssembly.GetTypes().Concat(projectAssembly.GetTypes())
				.Where(type => spriteType.IsAssignableFrom(type) && !type.IsAbstract)
				.Select(type => new SpriteType(type.FullName!)).ToList()));
	}

	public static List<string> GetScenePaths(string projectName) =>
		Directory.Exists($"build/{projectName}/assets/autogen/data/scenes")
			? Directory.GetFiles($"build/{projectName}/assets/autogen/data/scenes", "*.json",
				SearchOption.AllDirectories).ToList()
			: new();
}
