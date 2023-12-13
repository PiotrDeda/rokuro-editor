using System.Collections.Generic;

namespace Oirenomi.Models;

public class GameObject(string name, string sprite, string clazz, int x, int y)
{
	public string Name { get; set; } = name;
	public string Sprite { get; set; } = sprite;
	public string Class { get; set; } = clazz;
	public int X { get; set; } = x;
	public int Y { get; set; } = y;
	public List<(string, CustomProperty)> CustomProperties { get; set; } = new();
}
