using Rokuro.Dtos;

namespace RokuroEditor.Models;

public class Camera(string name, string clazz)
{
	public string Name { get; set; } = name;
	public string Class { get; set; } = clazz;

	public static Camera FromDto(CameraDto dto) => new(dto.Name, dto.Class);

	public CameraDto ToDto() => new(Name, Class);
}
