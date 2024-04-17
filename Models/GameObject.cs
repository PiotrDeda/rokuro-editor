using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using RokuroEditor.Dtos;

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
	SpriteType _spriteType = spriteType;
	string _camera = camera;
	int _positionX = positionX;
	int _positionY = positionY;
	double _scaleX = scaleX;
	double _scaleY = scaleY;
	double _rotation = rotation;
	bool _flipX = flipX;
	bool _flipY = flipY;

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

	public SpriteType SpriteType
	{
		get => _spriteType;
		set => this.RaiseAndSetIfChanged(ref _spriteType, value);
	}

	public string Sprite { get; set; } = sprite;

	public string Camera
	{
		get => _camera;
		set => this.RaiseAndSetIfChanged(ref _camera, value);
	}

	public int PositionX
	{
		get => _positionX;
		set => this.RaiseAndSetIfChanged(ref _positionX, value);
	}

	public int PositionY
	{
		get => _positionY;
		set => this.RaiseAndSetIfChanged(ref _positionY, value);
	}

	public double ScaleX
	{
		get => _scaleX;
		set => this.RaiseAndSetIfChanged(ref _scaleX, value);
	}

	public double ScaleY
	{
		get => _scaleY;
		set => this.RaiseAndSetIfChanged(ref _scaleY, value);
	}

	public double Rotation
	{
		get => _rotation;
		set => this.RaiseAndSetIfChanged(ref _rotation, value);
	}

	public bool FlipX
	{
		get => _flipX;
		set => this.RaiseAndSetIfChanged(ref _flipX, value);
	}

	public bool FlipY
	{
		get => _flipY;
		set => this.RaiseAndSetIfChanged(ref _flipY, value);
	}

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
