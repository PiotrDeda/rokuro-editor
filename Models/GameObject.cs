using System.Collections.ObjectModel;
using ReactiveUI;

namespace RokuroEditor.Models;

public class GameObject(string name, string sprite, string clazz, int x, int y) : ReactiveObject
{
	string _name = name;

	public string Name
	{
		get => _name;
		set => this.RaiseAndSetIfChanged(ref _name, value);
	}

	public string Sprite { get; set; } = sprite;
	public string Class { get; set; } = clazz;
	public int X { get; set; } = x;
	public int Y { get; set; } = y;
	public ObservableCollection<CustomProperty> CustomProperties { get; set; } = new();
}
