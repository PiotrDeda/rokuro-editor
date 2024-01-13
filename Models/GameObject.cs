using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using Rokuro.Dtos;

namespace RokuroEditor.Models;

public class GameObject(string name, string sprite, string clazz, string camera, int x, int y) : ReactiveObject
{
	string _name = name;

	public string Name
	{
		get => _name;
		set => this.RaiseAndSetIfChanged(ref _name, value);
	}

	public string Sprite { get; set; } = sprite;
	public string Class { get; set; } = clazz;
	public string Camera { get; set; } = camera;
	public int X { get; set; } = x;
	public int Y { get; set; } = y;
	public ObservableCollection<CustomProperty> CustomProperties { get; set; } = new();

	public static GameObject FromDto(GameObjectDto dto) =>
		new(dto.Name, dto.Sprite, dto.Class, dto.Camera, dto.X, dto.Y) {
			CustomProperties = new(dto.CustomProperties.Select(CustomProperty.FromDto))
		};

	public GameObjectDto ToDto() =>
		new(Name, Sprite, Class, Camera, X, Y, CustomProperties.Select(p => p.ToDto()).ToList());
}
