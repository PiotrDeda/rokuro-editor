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
	Camera? _selectedCamera;
	GameObject? _selectedGameObject;
	Scene? _selectedScene;

	public string? ProjectPath { get; set; }
	public string? ProjectName { get; set; }
	public string DotNetPath { get; set; } = "dotnet";
	public ObservableCollection<Scene> Scenes { get; set; } = new();

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

	public void SetProjectPathAndName(string? projectPath)
	{
		if (projectPath != null)
		{
			ProjectPath = projectPath.Split('\\').Take(projectPath.Split('\\').Length - 1)
				.Aggregate((a, b) => $"{a}\\{b}");
			ProjectName = projectPath.Split('\\').Last().Split('.').First();
		}
	}

	public bool BuildProject()
	{
		if (ProjectPath == null || ProjectName == null)
			return false;
		ProjectBuilder.Build(ProjectPath, ProjectName, DotNetPath);
		return true;
	}

	public bool LoadProject()
	{
		if (ProjectPath == null || ProjectName == null)
			return false;

		BuildProject();
		Scenes = new();
		ProjectBuilder.GetScenePaths(ProjectName).ForEach(scenePath =>
			Scenes.Add(Scene.FromDto(JsonConvert.DeserializeObject<SceneDto>(File.ReadAllText(scenePath))!)));

		this.RaisePropertyChanged(nameof(Scenes));
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

	public void LoadSampleData()
	{
		if (Scenes.Count > 0)
			return;

		ProjectPath = @"C:\Users\Example\Documents\Sample Project";
		ProjectName = "Sample Project";

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
