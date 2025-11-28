using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using RokuroEditor.Dtos;

namespace RokuroEditor.Models;

public class Camera(string name, CameraType clazz, ObservableCollection<CustomProperty> customProperties) : ReactiveObject
{
	public string Name
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = name;

	public CameraType Class { get; set; } = clazz;

	public ObservableCollection<CustomProperty> CustomProperties
	{
		get;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = customProperties;

	public static Camera FromDto(CameraDto dto, ProjectTypes types) =>
		new(
			dto.Name,
			types.CameraTypes.FirstOrDefault(t => t.Name == dto.Class) ?? new CameraType("null", new()),
			new(dto.CustomProperties.Select(CustomProperty.FromDto))
		);

	public CameraDto ToDto() => new(Name, Class.Name, CustomProperties.Select(p => p.ToDto()).ToList());
}
