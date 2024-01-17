using RokuroEditor.Models;

namespace RokuroEditor.ViewModels;

public class ConsolePanelViewModel(ProjectData projectData) : ViewModelBase
{
	public ConsolePanelViewModel() : this(new())
	{
		ProjectData.LoadSampleData();
	}

	public ProjectData ProjectData { get; set; } = projectData;
	public void ClearCommand() => ProjectData.ConsoleLog = "";
}
