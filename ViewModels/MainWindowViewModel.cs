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
		ConsolePanelViewModel = new(ProjectData);
	}

	public ListPanelViewModel ListPanelViewModel { get; }
	public MenuPanelViewModel MenuPanelViewModel { get; }
	public RunButtonsPanelViewModel RunButtonsPanelViewModel { get; }
	public PropertiesPanelViewModel PropertiesPanelViewModel { get; }
	public ConsolePanelViewModel ConsolePanelViewModel { get; }

	ProjectData ProjectData { get; } = new();
}
