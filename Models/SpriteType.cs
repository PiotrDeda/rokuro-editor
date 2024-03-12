using System.Linq;

namespace RokuroEditor.Models;

public class SpriteType(string name)
{
	public string Name { get; } = name;
	public string DisplayName => Name.Split('.').Last();
}
