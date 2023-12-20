using Avalonia.ReactiveUI;
using RokuroEditor.ViewModels;

namespace RokuroEditor.Views.Panels;

public partial class CameraPropertiesPanel : ReactiveUserControl<PropertiesPanelViewModel>
{
	public CameraPropertiesPanel()
	{
		InitializeComponent();
	}
}
