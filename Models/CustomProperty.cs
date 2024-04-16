using RokuroEditor.Dtos;

namespace RokuroEditor.Models;

public class CustomProperty(string name, string value)
{
	public string Name { get; set; } = name;
	public string Value { get; set; } = value;

	public static CustomProperty FromDto(CustomPropertyDto dto) => new(dto.Name, dto.Value);

	public CustomPropertyDto ToDto() => new(Name, Value);
}
