using Avalonia.ReactiveUI;
using RokuroEditor.ViewModels;

namespace RokuroEditor.Views.Panels;

public partial class ListPanel : ReactiveUserControl<ListPanelViewModel>
{
	public ListPanel()
	{
		InitializeComponent();
	}
}
