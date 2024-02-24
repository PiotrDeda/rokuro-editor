using System;
using System.Collections.Generic;
using System.Linq;

namespace RokuroEditor.Models;

public class GameObjectType(string name, List<string> customProperties)
{
	public static readonly List<string> ReservedProperties = new()
		{ "Enabled", "Position", "Sprite", "Camera", "WasMouseoverHandled", "Color", "Font" };

	public string Name { get; } = name;
	public List<string> CustomProperties { get; } = customProperties;

	public static GameObjectType FromType(Type type)
	{
		return new(type.FullName ?? "null",
			type.GetProperties()
				.Where(property => !ReservedProperties.Contains(property.Name))
				.Select(property => property.Name).ToList()
		);
	}
}
