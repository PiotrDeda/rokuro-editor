using System.Collections.Generic;
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

	public static Scene FromDto(SceneDto dto, List<GameObjectType> gameObjectTypes, List<SpriteType> spriteTypes) =>
		new(
			dto.Name,
			new(dto.GameObjects.Select(x => GameObject.FromDto(x, gameObjectTypes, spriteTypes))),
			new(dto.Cameras.Select(Camera.FromDto))
		);

	public SceneDto ToDto() => new(Name, GameObjects.Select(x => x.ToDto()).ToList(),
		Cameras.Select(x => x.ToDto()).ToList());
}
