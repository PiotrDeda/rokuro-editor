using System.Collections.Generic;

namespace RokuroEditor.Dtos;

public record GameObjectDto(
	string Name,
	string Class,
	string SpriteType,
	string Sprite,
	string Camera,
	int PositionX,
	int PositionY,
	double ScaleX,
	double ScaleY,
	double Rotation,
	bool FlipX,
	bool FlipY,
	List<CustomPropertyDto> CustomProperties
);
