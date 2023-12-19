using RokuroEditor.Models;

namespace RokuroEditor.ViewModels;

public class RunButtonsPanelViewModel(ProjectData projectData) : ViewModelBase
{
	public RunButtonsPanelViewModel() : this(new())
	{
		ProjectData.LoadSampleData();
	}

	public ProjectData ProjectData { get; } = projectData;
	public string FlyoutMessage { get; set; } = "No project is open";
}
