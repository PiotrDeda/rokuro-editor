using Avalonia;
using Avalonia.Controls.Primitives;

namespace Oirenomi.Views.Controls;

public class PropertyControl : TemplatedControl
{
	public static readonly StyledProperty<string> LabelProperty =
		AvaloniaProperty.Register<PropertyControl, string>(nameof(Label));

	public string Label
	{
		get => GetValue(LabelProperty);
		set => SetValue(LabelProperty, value);
	}
}
