using System.Collections.Generic;

namespace RokuroEditor.Dtos;

public record SceneDto(
	string Name,
	string Class,
	List<GameObjectDto> GameObjects,
	List<CameraDto> Cameras,
	List<CustomPropertyDto> CustomProperties
);
