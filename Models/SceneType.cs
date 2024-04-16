using System;
using System.Collections.Generic;
using System.Linq;

namespace RokuroEditor.Models;

public class SceneType(string name, List<string> customProperties)
{
	public static readonly List<string> ReservedProperties = new() { "Name" };

	public string Name { get; } = name;
	public List<string> CustomProperties { get; } = customProperties;
	public string DisplayName => Name.Split('.').Last();

	public static SceneType FromType(Type type)
	{
		return new(type.FullName ?? "null",
			type.GetProperties()
				.Where(property => !ReservedProperties.Contains(property.Name))
				.Select(property => property.Name).ToList()
		);
	}
}
