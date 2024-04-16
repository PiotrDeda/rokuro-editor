using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using RokuroEditor.Dtos;

namespace RokuroEditor.Models;

public class Camera(string name, CameraType clazz, ObservableCollection<CustomProperty> customProperties)
	: ReactiveObject
{
	ObservableCollection<CustomProperty> _customProperties = customProperties;
	string _name = name;

	public string Name
	{
		get => _name;
		set => this.RaiseAndSetIfChanged(ref _name, value);
	}

	public CameraType Class { get; set; } = clazz;

	public ObservableCollection<CustomProperty> CustomProperties
	{
		get => _customProperties;
		set => this.RaiseAndSetIfChanged(ref _customProperties, value);
	}

	public static Camera FromDto(CameraDto dto, ProjectTypes types) =>
		new(
			dto.Name,
			types.CameraTypes.FirstOrDefault(t => t.Name == dto.Class) ?? new CameraType("null", new()),
			new(dto.CustomProperties.Select(CustomProperty.FromDto))
		);

	public CameraDto ToDto() => new(Name, Class.Name, CustomProperties.Select(p => p.ToDto()).ToList());
}
