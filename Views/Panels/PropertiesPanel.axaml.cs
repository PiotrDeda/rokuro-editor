using Avalonia.ReactiveUI;
using Oirenomi.ViewModels;

namespace Oirenomi.Views.Panels;

public partial class PropertiesPanel : ReactiveUserControl<PropertiesPanelViewModel>
{
	public PropertiesPanel()
	{
		InitializeComponent();
	}
}
