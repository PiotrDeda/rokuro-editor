using ReactiveUI;
using Rokuro.Dtos;

namespace RokuroEditor.Models;

public class Camera(string name, string clazz) : ReactiveObject
{
	string _name = name;

	public string Name
	{
		get => _name;
		set => this.RaiseAndSetIfChanged(ref _name, value);
	}

	public string Class { get; set; } = clazz;

	public static Camera FromDto(CameraDto dto) => new(dto.Name, dto.Class);

	public CameraDto ToDto() => new(Name, Class);
}
