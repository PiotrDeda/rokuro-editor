using Avalonia.ReactiveUI;
using Oirenomi.ViewModels;

namespace Oirenomi.Views.Panels;

public partial class ListPanel : ReactiveUserControl<ListPanelViewModel>
{
	public ListPanel()
	{
		InitializeComponent();
	}
}
