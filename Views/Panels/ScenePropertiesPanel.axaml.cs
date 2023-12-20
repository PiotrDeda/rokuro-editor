using Avalonia.ReactiveUI;
using RokuroEditor.ViewModels;

namespace RokuroEditor.Views.Panels;

public partial class ScenePropertiesPanel : ReactiveUserControl<PropertiesPanelViewModel>
{
	public ScenePropertiesPanel()
	{
		InitializeComponent();
	}
}
