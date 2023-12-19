using Avalonia.ReactiveUI;
using RokuroEditor.ViewModels;

namespace RokuroEditor.Views.Panels;

public partial class PropertiesPanel : ReactiveUserControl<PropertiesPanelViewModel>
{
	public PropertiesPanel()
	{
		InitializeComponent();
	}
}
