using System;
using System.Collections.Generic;
using System.Linq;

namespace RokuroEditor.Models;

public class CameraType(string name, List<string> customProperties)
{
	static readonly List<string> ReservedProperties = ["Name", "Position", "Scale"];

	public string Name { get; } = name;
	public List<string> CustomProperties { get; } = customProperties;
	public string DisplayName => Name.Split('.').Last();

	public static CameraType FromType(Type type)
	{
		return new(type.FullName ?? "null",
			type.GetProperties()
				.Where(property => !ReservedProperties.Contains(property.Name))
				.Select(property => property.Name).ToList()
		);
	}
}
