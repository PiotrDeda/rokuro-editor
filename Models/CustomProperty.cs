namespace Oirenomi.Models;

public class CustomProperty(string name, string value)
{
	public string Name { get; set; } = name;
	public string Value { get; set; } = value;
}
