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
		try
		{
			using (var process = new Process())
			{
				process.StartInfo = new() {
					FileName = Cmd,
					Arguments =
						$"{ArgBegin}{dotNetPath} new sln --name \"{projectName}\" --output \"{projectPath}\" && " +
						$"{dotNetPath} new console --name \"{projectName}\" --output \"{projectPath}\" && " +
						$"cd {projectPath} && " +
						$"{dotNetPath} sln add \"{projectPath}/{projectName}.csproj\" && " +
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

			File.Copy("assets_editor/templates/Program.cstemplate", $"{projectPath}/Program.cs", true);
			string sanitizedProjectName = projectName;
			Regex.Replace(sanitizedProjectName, @"\s+", "");
			File.WriteAllLines($"{projectPath}/Program.cs", File.ReadAllLines($"{projectPath}/Program.cs")
				.Select(line => line.Replace("%{ProjectName}%", sanitizedProjectName))
			);
		}
		catch (Exception e)
		{
			log(e.ToString());
		}
	}

	public static void Build(string projectPath, string projectName, string dotNetPath, Action<string> log)
	{
		try
		{
			using (var process = new Process())
			{
				process.StartInfo = new() {
					FileName = Cmd,
					Arguments =
						$"{ArgBegin}{dotNetPath} build \"{projectPath}/{projectName}.csproj\" --output build/{projectName}{ArgEnd}",
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
		catch (Exception e)
		{
			log(e.ToString());
		}
	}

	public static Process Run(Process process, string projectName, Action<string> log)
	{
		process.StartInfo.FileName = $"build/{projectName}/{projectName}{Ext}";
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

		Type gameObjectType = rokuroAssembly.GetType("Rokuro.Objects.GameObject")!;
		ObservableCollection<GameObjectType> gameObjectTypes = new(rokuroAssembly.GetTypes()
			.Concat(projectAssembly.GetTypes()).Where(type => gameObjectType.IsAssignableFrom(type) && !type.IsAbstract)
			.Select(type => GameObjectType.FromType(type)).OrderBy(type => type.DisplayName).ToList());

		Type cameraType = rokuroAssembly.GetType("Rokuro.Graphics.Camera")!;
		ObservableCollection<CameraType> cameraTypes = new(rokuroAssembly.GetTypes()
			.Concat(projectAssembly.GetTypes()).Where(type => cameraType.IsAssignableFrom(type) && !type.IsAbstract)
			.Select(type => CameraType.FromType(type)).OrderBy(type => type.DisplayName).ToList());

		Type spriteType = rokuroAssembly.GetType("Rokuro.Graphics.Sprite")!;
		ObservableCollection<SpriteType> spriteTypes = new(rokuroAssembly.GetTypes()
			.Concat(projectAssembly.GetTypes()).Where(type => spriteType.IsAssignableFrom(type) && !type.IsAbstract)
			.Select(type => new SpriteType(type.FullName!)).OrderBy(type => type.DisplayName).ToList());

		Type sceneType = rokuroAssembly.GetType("Rokuro.Graphics.Scene")!;
		ObservableCollection<SceneType> sceneTypes = new(rokuroAssembly.GetTypes()
			.Concat(projectAssembly.GetTypes()).Where(type => sceneType.IsAssignableFrom(type) && !type.IsAbstract)
			.Select(type => SceneType.FromType(type)).OrderBy(type => type.DisplayName).ToList());

		return new(gameObjectTypes, cameraTypes, spriteTypes, sceneTypes);
	}

	public static List<string> GetScenePaths(string projectName) =>
		Directory.Exists($"build/{projectName}/assets/autogen/data/scenes")
			? Directory.GetFiles($"build/{projectName}/assets/autogen/data/scenes", "*.json",
				SearchOption.AllDirectories).ToList()
			: new();
}
