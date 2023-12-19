using System.Collections.ObjectModel;

namespace RokuroEditor.Models;

public class Scene(string name)
{
	public string Name { get; set; } = name;
	public ObservableCollection<GameObject> GameObjects { get; set; } = new();
	public ObservableCollection<Camera> Cameras { get; set; } = new();
}
