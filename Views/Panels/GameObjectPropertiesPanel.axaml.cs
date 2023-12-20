using Avalonia.ReactiveUI;
using RokuroEditor.ViewModels;

namespace RokuroEditor.Views.Panels;

public partial class GameObjectPropertiesPanel : ReactiveUserControl<PropertiesPanelViewModel>
{
	public GameObjectPropertiesPanel()
	{
		InitializeComponent();
	}
}
