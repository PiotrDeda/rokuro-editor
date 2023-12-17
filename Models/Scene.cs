using System.Collections.Generic;

namespace Oirenomi.Models;

public class Scene(string name)
{
	public string Name { get; set; } = name;
	public List<GameObject> GameObjects { get; set; } = new();
	public List<Camera> Cameras { get; set; } = new();
}
