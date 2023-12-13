using Oirenomi.Models;

namespace Oirenomi.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
	public MainWindowViewModel()
	{
		RunButtonsPanelViewModel = new(ProjectData);
		MenuPanelViewModel = new(ProjectData);
	}

	public MenuPanelViewModel MenuPanelViewModel { get; }
	public RunButtonsPanelViewModel RunButtonsPanelViewModel { get; }

	ProjectData ProjectData { get; } = new();
}
