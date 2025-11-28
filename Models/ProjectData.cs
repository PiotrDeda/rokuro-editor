using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ReactiveUI;
using RokuroEditor.Dtos;

namespace RokuroEditor.Models;

public class ProjectData : ReactiveObject
{
	public string? ProjectPath { get; set; }
	public string? ProjectName { get; set; }
	public string DotNetPath { get; set; } = "dotnet";

	public ObservableCollection<Scene> Scenes
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = [];

	public string ConsoleLog
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = "";

	public Scene? SelectedScene
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	}

	public GameObject? SelectedGameObject
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	}

	public Camera? SelectedCamera
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	}

	public ProjectTypes ProjectTypes
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = new();

	public void Log(string message) => ConsoleLog += message;

	public void SetProjectPathAndName(string? projectPath)
	{
		if (projectPath == null)
			return;
		ProjectPath = projectPath.Split(Path.DirectorySeparatorChar)
			.Take(projectPath.Split(Path.DirectorySeparatorChar).Length - 1)
			.Aggregate((a, b) => $"{a}{Path.DirectorySeparatorChar}{b}");
		ProjectName = projectPath.Split(Path.DirectorySeparatorChar).Last().Split('.').First();
	}

	public bool NewProject()
	{
		if (ProjectPath == null || ProjectName == null)
			return false;

		if (Directory.EnumerateFileSystemEntries(ProjectPath).Any())
		{
			Log($"Selected directory (\"{ProjectPath}\") is not empty");
			return false;
		}

		try
		{
			ProjectBuilder.CreateProject(ProjectPath, ProjectName, DotNetPath, Log);
		}
		catch (Exception e)
		{
			Log(e.ToString());
			return false;
		}

		return true;
	}

	public bool BuildProject()
	{
		if (ProjectPath == null || ProjectName == null)
			return false;

		try
		{
			ProjectBuilder.Build(ProjectPath, ProjectName, DotNetPath, Log);
		}
		catch (Exception e)
		{
			Log(e.ToString());
			return false;
		}

		return true;
	}

	public void LoadProject()
	{
		if (ProjectPath == null || ProjectName == null)
			return;

		try
		{
			ProjectTypes = ProjectBuilder.GetTypes(ProjectName);
			Scenes = [];
			ProjectBuilder.GetScenePaths(ProjectPath).ForEach(scenePath =>
				Scenes.Add(Scene.FromDto(JsonConvert.DeserializeObject<SceneDto>(File.ReadAllText(scenePath))!, ProjectTypes)));
		}
		catch (Exception e)
		{
			Log(e.ToString());
		}
	}

	public void SaveProject()
	{
		if (ProjectPath == null || ProjectName == null)
			return;

		try
		{
			string scenesPath = Path.Combine(ProjectPath, "assets", "autogen", "scenes");
			Directory.CreateDirectory(scenesPath);
			Array.ForEach(Directory.GetFiles(scenesPath), File.Delete);
			Scenes.ToList().ForEach(scene => File.WriteAllText(Path.Combine(scenesPath, $"{scene.Name}.json"), JsonConvert.SerializeObject(scene.ToDto())));
		}
		catch (Exception e)
		{
			Log(e.ToString());
		}
	}

	public void CloseProject()
	{
		ProjectPath = null;
		ProjectName = null;
		Scenes = [];
		SelectedScene = null;
		SelectedGameObject = null;
		SelectedCamera = null;
		ConsoleLog = "";
		ProjectTypes = new();
	}

	public void ClearLoadedData()
	{
		Scenes = [];
		SelectedScene = null;
		SelectedGameObject = null;
		SelectedCamera = null;
		ProjectTypes = new();
	}

	public void AddScene()
	{
		Scenes.Add(new("New Scene",
			ProjectTypes.SceneTypes.Single(t => t.Name == "Rokuro.Objects.Scene"),
			[], [], []));
		SelectedScene = Scenes.Last();
	}

	public void AddGameObject()
	{
		if (SelectedScene == null)
			return;

		SelectedScene.GameObjects.Add(new("New Object",
			ProjectTypes.GameObjectTypes.Single(t => t.Name == "Rokuro.Objects.GameObject"),
			ProjectTypes.SpriteTypes.Single(t => t.Name == "Rokuro.Graphics.StaticSprite"),
			"", "Camera", 0, 0, 1, 1, 0, false, false, []));
		SelectedGameObject = SelectedScene.GameObjects.Last();
	}

	public void AddCamera()
	{
		if (SelectedScene == null)
			return;

		SelectedScene.Cameras.Add(new("New Camera",
			ProjectTypes.CameraTypes.Single(t => t.Name == "Rokuro.Graphics.Camera"),
			[]));
		SelectedCamera = SelectedScene.Cameras.Last();
	}

	public void DeleteSelectedScene()
	{
		if (SelectedScene == null)
			return;

		Scenes.Remove(SelectedScene);
		SelectedScene = null;
	}

	public void DeleteSelectedGameObject()
	{
		if (SelectedScene == null || SelectedGameObject == null)
			return;

		SelectedScene.GameObjects.Remove(SelectedGameObject);
		SelectedGameObject = null;
	}

	public void DeleteSelectedCamera()
	{
		if (SelectedScene == null || SelectedCamera == null)
			return;

		SelectedScene.Cameras.Remove(SelectedCamera);
		SelectedCamera = null;
	}

	public void LoadSampleData()
	{
		if (Scenes.Count > 0)
			return;

		ProjectPath = @"C:\Users\Example\Documents\Sample Project";
		ProjectName = "Sample Project";
		for (int i = 0; i < 100; i++)
			ConsoleLog += $"Sample console log line {i}\n";

		// = Scenes =
		Scenes = [
			new("Game Example", new("Rokuro.Objects.Scene", []), [], [], []),
			new("Many Objects", new("Rokuro.Objects.Scene", []), [], [], []),
			new("Empty", new("Rokuro.Objects.Scene", []), [], [], [])
		];

		// = Cameras =
		// == Scene 0 ==
		Scenes[0].Cameras.Add(new("Camera", new("Rokuro.Graphics.Camera", []), []));
		Scenes[0].Cameras.Add(new("UI Camera", new("Rokuro.Graphics.UICamera", []), []));
		// == Scene 1 ==
		foreach (int i in Enumerable.Range(0, 100))
			Scenes[1].Cameras.Add(new($"Camera{i}", new("Rokuro.Graphics.Camera", []), []));

		// = GameObjects =
		// == Scene 0 ==
		Scenes[0].GameObjects.Add(new("Player", new("Rokuro.Player", []), new("Rokuro.Graphics.StaticSprite"),
			"tiles/player", "Camera", 0, 0, 1, 1, 0, false, false, []));
		Scenes[0].GameObjects[0].CustomProperties.Add(new("Health", "10"));
		Scenes[0].GameObjects[0].CustomProperties.Add(new("Damage", "1"));
		Scenes[0].GameObjects.Add(new("Enemy", new("Rokuro.Enemies.Enemy", []), new("Rokuro.Graphics.StaticSprite"),
			"tiles/enemy", "Camera", 0, 0, 1, 1, 0, false, false, []));
		Scenes[0].GameObjects.Add(new("Item", new("Rokuro.Items.Item", []), new("Rokuro.Graphics.StaticSprite"),
			"tiles/item", "UI Camera", 0, 0, 1, 1, 0, false, false, []));
		// == Scene 1 ==
		foreach (int i in Enumerable.Range(0, 100))
			Scenes[1].GameObjects.Add(new($"GameObject{i}", new("Rokuro.Objects.GameObject", []),
				new("Rokuro.Graphics.StaticSprite"), "items/blank_item", $"Camera{i}",
				0, 0, 1, 1, 0, false, false, []));
		foreach (int i in Enumerable.Range(0, 100))
			Scenes[1].GameObjects[0].CustomProperties.Add(new($"Property{i}", $"Value{i}"));
	}
}
