using Oirenomi.Models;

namespace Oirenomi.ViewModels;

public class RunButtonsPanelViewModel(ProjectData projectData) : ViewModelBase
{
	public ProjectData ProjectData { get; } = projectData;
	public string FlyoutMessage { get; set; } = "No project is open";
}
