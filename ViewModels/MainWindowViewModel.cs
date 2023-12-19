using RokuroEditor.Models;

namespace RokuroEditor.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
	public MainWindowViewModel()
	{
		ListPanelViewModel = new(ProjectData);
		RunButtonsPanelViewModel = new(ProjectData);
		MenuPanelViewModel = new(ProjectData);
		PropertiesPanelViewModel = new(ProjectData);
	}

	public ListPanelViewModel ListPanelViewModel { get; }
	public MenuPanelViewModel MenuPanelViewModel { get; }
	public RunButtonsPanelViewModel RunButtonsPanelViewModel { get; }
	public PropertiesPanelViewModel PropertiesPanelViewModel { get; }

	ProjectData ProjectData { get; } = new();
}
