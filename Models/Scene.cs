using System.Collections.Generic;

namespace Oirenomi.Models;

public class Scene
{
	List<GameObject> GameObjects { get; set; } = new();
	List<Camera> Cameras { get; set; } = new();
}
