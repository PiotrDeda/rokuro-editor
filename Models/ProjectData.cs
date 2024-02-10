using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ReactiveUI;
using Rokuro.Dtos;

namespace RokuroEditor.Models;

public class ProjectData : ReactiveObject
{
	string _consoleLog = "";
	ObservableCollection<Scene> _scenes = new();
	Camera? _selectedCamera;
	GameObject? _selectedGameObject;
	Scene? _selectedScene;

	public string? ProjectPath { get; set; }
	public string? ProjectName { get; set; }
	public string DotNetPath { get; set; } = "dotnet";

	public ObservableCollection<Scene> Scenes
	{
		get => _scenes;
		set => this.RaiseAndSetIfChanged(ref _scenes, value);
	}

	public string ConsoleLog
	{
		get => _consoleLog;
		set => this.RaiseAndSetIfChanged(ref _consoleLog, value);
	}

	public Scene? SelectedScene
	{
		get => _selectedScene;
		set => this.RaiseAndSetIfChanged(ref _selectedScene, value);
	}

	public GameObject? SelectedGameObject
	{
		get => _selectedGameObject;
		set => this.RaiseAndSetIfChanged(ref _selectedGameObject, value);
	}

	public Camera? SelectedCamera
	{
		get => _selectedCamera;
		set => this.RaiseAndSetIfChanged(ref _selectedCamera, value);
	}

	public void Log(string message) => ConsoleLog += message;

	public void SetProjectPathAndName(string? projectPath)
	{
		if (projectPath != null)
		{
			ProjectPath = projectPath.Split('\\').Take(projectPath.Split('\\').Length - 1)
				.Aggregate((a, b) => $"{a}\\{b}");
			ProjectName = projectPath.Split('\\').Last().Split('.').First();
		}
	}

	public bool NewProject()
	{
		if (ProjectPath == null || ProjectName == null)
			return false;

		if (Directory.EnumerateFileSystemEntries(ProjectPath).Any())
			throw new($"Selected directory (\"{ProjectPath}\") is not empty");

		ProjectBuilder.CreateProject(ProjectPath, ProjectName, DotNetPath, Log);

		return true;
	}

	public bool BuildProject()
	{
		if (ProjectPath == null || ProjectName == null)
			return false;
		ProjectBuilder.Build(ProjectPath, ProjectName, DotNetPath, Log);
		return true;
	}

	public bool LoadProject()
	{
		if (ProjectPath == null || ProjectName == null || BuildProject() == false)
			return false;

		Log("= Loaded types:\n");
		ProjectBuilder.GetGameObjectTypes(ProjectName).ForEach(type => Log($"= - {type.Name}\n"));
		Scenes = new();
		ProjectBuilder.GetScenePaths(ProjectName).ForEach(scenePath =>
			Scenes.Add(Scene.FromDto(JsonConvert.DeserializeObject<SceneDto>(File.ReadAllText(scenePath))!)));

		return true;
	}

	public bool SaveProject()
	{
		if (ProjectPath == null || ProjectName == null)
			return false;

		Directory.CreateDirectory($"{ProjectPath}/assets/autogen/data/scenes");
		Array.ForEach(Directory.GetFiles($"{ProjectPath}/assets/autogen/data/scenes"), File.Delete);

		Scenes.ToList().ForEach(scene =>
			File.WriteAllText($"{ProjectPath}/assets/autogen/data/scenes/{scene.Name}.json",
				JsonConvert.SerializeObject(scene.ToDto())));

		return true;
	}

	public void CloseProject()
	{
		ProjectPath = null;
		ProjectName = null;
		Scenes = new();
		ConsoleLog = "";
	}

	public void AddScene()
	{
		Scenes.Add(new("New Scene"));
		SelectedScene = Scenes.Last();
	}

	public void AddGameObject()
	{
		if (SelectedScene == null)
			return;

		SelectedScene.GameObjects.Add(new("New Object", "", "Rokuro.Objects.GameObject", "Camera", 0, 0));
		SelectedGameObject = SelectedScene.GameObjects.Last();
	}

	public void AddCamera()
	{
		if (SelectedScene == null)
			return;

		SelectedScene.Cameras.Add(new("New Camera", "Rokuro.Graphics.Camera"));
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
		Scenes = new() { new("Game Example"), new("Many Objects"), new("Empty") };

		// = Cameras =
		// == Scene 0 ==
		Scenes[0].Cameras.Add(new("Camera", "Rokuro.Graphics.Camera"));
		Scenes[0].Cameras.Add(new("UI Camera", "Rokuro.Graphics.UICamera"));
		// == Scene 1 ==
		foreach (int i in Enumerable.Range(0, 100))
			Scenes[1].Cameras.Add(new($"Camera{i}", "Rokuro.Graphics.Camera"));

		// = GameObjects =
		// == Scene 0 ==
		Scenes[0].GameObjects.Add(new("Player", "tiles/player", "Rokuro.Player", "Camera", 0, 0));
		Scenes[0].GameObjects[0].CustomProperties.Add(new("Health", "10"));
		Scenes[0].GameObjects[0].CustomProperties.Add(new("Damage", "1"));
		Scenes[0].GameObjects.Add(new("Enemy", "tiles/enemy", "Rokuro.Enemies.Enemy", "Camera", 0, 0));
		Scenes[0].GameObjects.Add(new("Item", "tiles/item", "Rokuro.Items.Item", "UI Camera", 0, 0));
		// == Scene 1 ==
		foreach (int i in Enumerable.Range(0, 100))
			Scenes[1].GameObjects.Add(new($"GameObject{i}", "GameObject", "Rokuro.GameObject", $"Camera{i}", 0, 0));
		foreach (int i in Enumerable.Range(0, 100))
			Scenes[1].GameObjects[0].CustomProperties.Add(new($"Property{i}", $"Value{i}"));
	}
}
