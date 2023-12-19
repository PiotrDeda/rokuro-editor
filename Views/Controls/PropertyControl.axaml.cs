using Avalonia;
using Avalonia.Controls.Primitives;

namespace RokuroEditor.Views.Controls;

public class PropertyControl : TemplatedControl
{
	public static readonly StyledProperty<string> LabelProperty =
		AvaloniaProperty.Register<PropertyControl, string>(nameof(Label));

	public static readonly StyledProperty<string> ValueProperty =
		AvaloniaProperty.Register<PropertyControl, string>(nameof(Value));

	public string Label
	{
		get => GetValue(LabelProperty);
		set => SetValue(LabelProperty, value);
	}

	public string Value
	{
		get => GetValue(ValueProperty);
		set => SetValue(ValueProperty, value);
	}
}
