using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using Rokuro.Dtos;

namespace RokuroEditor.Models;

public class Scene(string name, ObservableCollection<GameObject> gameObjects, ObservableCollection<Camera> cameras)
	: ReactiveObject
{
	string _name = name;

	public string Name
	{
		get => _name;
		set => this.RaiseAndSetIfChanged(ref _name, value);
	}

	public ObservableCollection<GameObject> GameObjects { get; set; } = gameObjects;
	public ObservableCollection<Camera> Cameras { get; set; } = cameras;

	public static Scene FromDto(SceneDto dto, ProjectTypes types) =>
		new(
			dto.Name,
			new(dto.GameObjects.Select(gameObjectDto => GameObject.FromDto(gameObjectDto, types))),
			new(dto.Cameras.Select(cameraDto => Camera.FromDto(cameraDto, types)))
		);

	public SceneDto ToDto() => new(Name, GameObjects.Select(go => go.ToDto()).ToList(),
		Cameras.Select(c => c.ToDto()).ToList());
}
