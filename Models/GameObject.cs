using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using Rokuro.Dtos;

namespace RokuroEditor.Models;

public class GameObject(string name, string sprite, GameObjectType clazz, string camera, int x, int y) : ReactiveObject
{
	GameObjectType _class = clazz;
	ObservableCollection<CustomProperty> _customProperties = new();
	string _name = name;

	public string Name
	{
		get => _name;
		set => this.RaiseAndSetIfChanged(ref _name, value);
	}

	public string Sprite { get; set; } = sprite;

	public GameObjectType Class
	{
		get => _class;
		set
		{
			_class = value;
			CustomProperties = new(value.CustomProperties.Select(p => new CustomProperty(p, "")).ToList());
		}
	}

	public string Camera { get; set; } = camera;
	public int X { get; set; } = x;
	public int Y { get; set; } = y;

	public ObservableCollection<CustomProperty> CustomProperties
	{
		get => _customProperties;
		set => this.RaiseAndSetIfChanged(ref _customProperties, value);
	}

	public static GameObject FromDto(GameObjectDto dto, List<GameObjectType> types) =>
		new(dto.Name, dto.Sprite, types.FirstOrDefault(t => t.Name == dto.Class) ?? new GameObjectType("null", new()),
			dto.Camera, dto.X, dto.Y) {
			CustomProperties = new(dto.CustomProperties.Select(CustomProperty.FromDto))
		};

	public GameObjectDto ToDto() =>
		new(Name, Sprite, Class.Name, Camera, X, Y, CustomProperties.Select(p => p.ToDto()).ToList());
}
