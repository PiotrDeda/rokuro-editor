using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using Rokuro.Dtos;

namespace RokuroEditor.Models;

public class GameObject(
	string name,
	GameObjectType clazz,
	SpriteType spriteType,
	string sprite,
	string camera,
	int positionX,
	int positionY,
	double scaleX,
	double scaleY,
	double rotation,
	bool flipX,
	bool flipY,
	ObservableCollection<CustomProperty> customProperties) : ReactiveObject
{
	GameObjectType _class = clazz;
	ObservableCollection<CustomProperty> _customProperties = customProperties;
	string _name = name;

	public string Name
	{
		get => _name;
		set => this.RaiseAndSetIfChanged(ref _name, value);
	}

	public GameObjectType Class
	{
		get => _class;
		set
		{
			_class = value;
			CustomProperties = new(value.CustomProperties.Select(p => new CustomProperty(p, "")).ToList());
		}
	}

	public SpriteType SpriteType { get; set; } = spriteType;
	public string Sprite { get; set; } = sprite;
	public string Camera { get; set; } = camera;
	public int PositionX { get; set; } = positionX;
	public int PositionY { get; set; } = positionY;
	public double ScaleX { get; set; } = scaleX;
	public double ScaleY { get; set; } = scaleY;
	public double Rotation { get; set; } = rotation;
	public bool FlipX { get; set; } = flipX;
	public bool FlipY { get; set; } = flipY;

	public ObservableCollection<CustomProperty> CustomProperties
	{
		get => _customProperties;
		set => this.RaiseAndSetIfChanged(ref _customProperties, value);
	}

	public static GameObject FromDto(GameObjectDto dto, ProjectTypes types) =>
		new(
			dto.Name,
			types.GameObjectTypes.FirstOrDefault(t => t.Name == dto.Class) ?? new GameObjectType("null", new()),
			types.SpriteTypes.FirstOrDefault(t => t.Name == dto.SpriteType) ?? new SpriteType("null"),
			dto.Sprite, dto.Camera, dto.PositionX, dto.PositionY, dto.ScaleX, dto.ScaleY, dto.Rotation,
			dto.FlipX, dto.FlipY, new(dto.CustomProperties.Select(CustomProperty.FromDto))
		);

	public GameObjectDto ToDto()
	{
		string spriteType = Class.Name == "Rokuro.Objects.TextObject" ? "Rokuro.Graphics.TextSprite" : SpriteType.Name;
		return new(Name, Class.Name, spriteType, Sprite, Camera, PositionX, PositionY, ScaleX, ScaleY, Rotation,
			FlipX, FlipY, CustomProperties.Select(p => p.ToDto()).ToList());
	}
}
