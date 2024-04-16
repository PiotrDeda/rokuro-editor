using System.Collections.Generic;

namespace RokuroEditor.Dtos;

public record CameraDto(string Name, string Class, List<CustomPropertyDto> CustomProperties);
