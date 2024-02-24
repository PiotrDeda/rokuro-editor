using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using Rokuro.Dtos;

namespace RokuroEditor.Models;

public class Scene(string name) : ReactiveObject
{
	string _name = name;

	public string Name
	{
		get => _name;
		set => this.RaiseAndSetIfChanged(ref _name, value);
	}

	public ObservableCollection<GameObject> GameObjects { get; set; } = new();
	public ObservableCollection<Camera> Cameras { get; set; } = new();

	public static Scene FromDto(SceneDto dto, List<GameObjectType> types) => new(dto.Name) {
		GameObjects = new(dto.GameObjects.Select(x => GameObject.FromDto(x, types))),
		Cameras = new(dto.Cameras.Select(Camera.FromDto))
	};

	public SceneDto ToDto() => new(Name, GameObjects.Select(x => x.ToDto()).ToList(),
		Cameras.Select(x => x.ToDto()).ToList());
}
