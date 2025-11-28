using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using RokuroEditor.Models;

namespace RokuroEditor;

public static class ProjectBuilder
{
	static readonly string Cmd = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "bash";
	static readonly string ArgBegin = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "/C " : "-c \"";
	static readonly string ArgEnd = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "" : "\"";
	static readonly string Ext = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "";

	public static void CreateProject(string projectPath, string projectName, string dotNetPath, Action<string> log)
	{
		using (var process = new Process())
		{
			process.StartInfo = new() {
				FileName = Cmd,
				Arguments =
					$"{ArgBegin}{dotNetPath} new sln --name \"{projectName}\" --output \"{projectPath}\" && " +
					$"{dotNetPath} new console --name \"{projectName}\" --output \"{projectPath}\" && " +
					$"cd {projectPath} && " +
					$"{dotNetPath} sln add \"{projectName}.csproj\" && " +
					$"{dotNetPath} add package Sayers.SDL2.Core --version 1.0.11 && " +
					$"{dotNetPath} nuget add source https://f.feedz.io/rokuro/rokuro/nuget/index.json || " +
					$"{dotNetPath} add package Rokuro && " +
					$"{dotNetPath} restore{ArgEnd}",
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

		File.Copy(Path.Combine("assets_editor", "templates", "Program.cstemplate"),
			Path.Combine(projectPath, "Program.cs"), true);
		string sanitizedProjectName = projectName;
		Regex.Replace(sanitizedProjectName, @"\s+", "");
		File.WriteAllLines(Path.Combine(projectPath, "Program.cs"), File.ReadAllLines(Path.Combine(projectPath, "Program.cs"))
			.Select(line => line.Replace("%{ProjectName}%", sanitizedProjectName))
		);
	}

	public static void Build(string projectPath, string projectName, string dotNetPath, Action<string> log)
	{
		using var process = new Process();
		process.StartInfo = new() {
			FileName = Cmd,
			Arguments =
				$"{ArgBegin}{dotNetPath} build \"{Path.Combine(projectPath, projectName)}.csproj\" --output {Path.Combine("build", projectName)}{ArgEnd}",
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

	public static Process Run(Process process, string projectName, Action<string> log)
	{
		process.StartInfo.FileName = Path.Combine("build", projectName, $"{projectName}{Ext}");
		process.StartInfo.WorkingDirectory = Path.Combine("build", projectName);
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
		string projectAssemblyPath = Path.Combine("build", projectName, $"{projectName}.dll");
		string rokuroAssemblyPath = Path.Combine("build", projectName, "Rokuro.dll");
		var resolver = new PathAssemblyResolver(new List<string> {
			projectAssemblyPath,
			rokuroAssemblyPath,
			typeof(object).Assembly.Location,
			typeof(object).Assembly.Location.Replace("System.Private.CoreLib.dll", "System.Runtime.dll")
		});
		using var mlc = new MetadataLoadContext(resolver, typeof(object).Assembly.GetName().ToString());
		Assembly projectAssembly = mlc.LoadFromAssemblyPath(projectAssemblyPath);
		Assembly rokuroAssembly = mlc.LoadFromAssemblyPath(rokuroAssemblyPath);

		Type gameObjectType = rokuroAssembly.GetType("Rokuro.Objects.GameObject")!;
		ObservableCollection<GameObjectType> gameObjectTypes = new(rokuroAssembly.GetTypes()
			.Concat(projectAssembly.GetTypes()).Where(type => gameObjectType.IsAssignableFrom(type) && !type.IsAbstract)
			.Select(GameObjectType.FromType).OrderBy(type => type.DisplayName).ToList());

		Type cameraType = rokuroAssembly.GetType("Rokuro.Graphics.Camera")!;
		ObservableCollection<CameraType> cameraTypes = new(rokuroAssembly.GetTypes()
			.Concat(projectAssembly.GetTypes()).Where(type => cameraType.IsAssignableFrom(type) && !type.IsAbstract)
			.Select(CameraType.FromType).OrderBy(type => type.DisplayName).ToList());

		Type spriteType = rokuroAssembly.GetType("Rokuro.Graphics.Sprite")!;
		ObservableCollection<SpriteType> spriteTypes = new(rokuroAssembly.GetTypes()
			.Concat(projectAssembly.GetTypes()).Where(type => spriteType.IsAssignableFrom(type) && !type.IsAbstract)
			.Select(type => new SpriteType(type.FullName!)).OrderBy(type => type.DisplayName).ToList());

		Type sceneType = rokuroAssembly.GetType("Rokuro.Objects.Scene")!;
		ObservableCollection<SceneType> sceneTypes = new(rokuroAssembly.GetTypes()
			.Concat(projectAssembly.GetTypes()).Where(type => sceneType.IsAssignableFrom(type) && !type.IsAbstract)
			.Select(SceneType.FromType).OrderBy(type => type.DisplayName).ToList());

		return new(gameObjectTypes, cameraTypes, spriteTypes, sceneTypes);
	}

	public static List<string> GetScenePaths(string projectPath) =>
		Directory.Exists(Path.Combine(projectPath, "assets", "autogen", "scenes"))
			? Directory.GetFiles(Path.Combine(projectPath, "assets", "autogen", "scenes"), "*.json", SearchOption.AllDirectories).ToList()
			: [];
}
