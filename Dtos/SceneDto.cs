using System.Collections.Generic;

namespace RokuroEditor.Dtos;

public record SceneDto(string Name, List<GameObjectDto> GameObjects, List<CameraDto> Cameras);
