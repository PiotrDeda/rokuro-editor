using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace RokuroEditor.Views.Panels;

public partial class ConsolePanel : UserControl
{
	public ConsolePanel()
	{
		InitializeComponent();
		TextBox = this.FindControl<TextBox>("ConsoleTextBox");
		if (TextBox != null)
			TextBox.PropertyChanged += TextBoxPropertyChanged;
	}

	TextBox? TextBox { get; }
	ScrollViewer? ScrollViewer { get; set; }

	void TextBoxPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property.Name == "Text")
		{
			ScrollViewer ??= TextBox?.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();
			ScrollViewer?.ScrollToEnd();
		}
	}
}
