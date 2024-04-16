using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using RokuroEditor.Dtos;

namespace RokuroEditor.Models;

public class Scene(
	string name,
	SceneType clazz,
	ObservableCollection<GameObject> gameObjects,
	ObservableCollection<Camera> cameras,
	ObservableCollection<CustomProperty> customProperties
) : ReactiveObject
{
	ObservableCollection<CustomProperty> _customProperties = customProperties;
	string _name = name;

	public string Name
	{
		get => _name;
		set => this.RaiseAndSetIfChanged(ref _name, value);
	}

	public SceneType Class { get; set; } = clazz;
	public ObservableCollection<GameObject> GameObjects { get; set; } = gameObjects;
	public ObservableCollection<Camera> Cameras { get; set; } = cameras;

	public ObservableCollection<CustomProperty> CustomProperties
	{
		get => _customProperties;
		set => this.RaiseAndSetIfChanged(ref _customProperties, value);
	}

	public static Scene FromDto(SceneDto dto, ProjectTypes types) =>
		new(
			dto.Name,
			types.SceneTypes.FirstOrDefault(t => t.Name == dto.Class) ?? new SceneType("null", new()),
			new(dto.GameObjects.Select(gameObjectDto => GameObject.FromDto(gameObjectDto, types))),
			new(dto.Cameras.Select(cameraDto => Camera.FromDto(cameraDto, types))),
			new(dto.CustomProperties.Select(CustomProperty.FromDto))
		);

	public SceneDto ToDto() => new(Name, Class.Name, GameObjects.Select(go => go.ToDto()).ToList(),
		Cameras.Select(c => c.ToDto()).ToList(), CustomProperties.Select(p => p.ToDto()).ToList());
}
