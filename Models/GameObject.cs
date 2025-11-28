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
	public string Name
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = name;

	public GameObjectType Class
	{
		get;
		set
		{
			field = value;
			CustomProperties = new(value.CustomProperties.Select(p => new CustomProperty(p, "")).ToList());
		}
	} = clazz;

	public SpriteType SpriteType
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = spriteType;

	public string Sprite { get; set; } = sprite;

	public string Camera
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = camera;

	public int PositionX
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = positionX;

	public int PositionY
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = positionY;

	public double ScaleX
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = scaleX;

	public double ScaleY
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = scaleY;

	public double Rotation
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = rotation;

	public bool FlipX
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = flipX;

	public bool FlipY
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = flipY;

	public ObservableCollection<CustomProperty> CustomProperties
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = customProperties;

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
