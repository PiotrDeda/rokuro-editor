using RokuroEditor.Models;

namespace RokuroEditor.ViewModels;

public class ListPanelViewModel(ProjectData projectData) : ViewModelBase
{
	public ListPanelViewModel() : this(new())
	{
		ProjectData.LoadSampleData();
	}

	public ProjectData ProjectData { get; set; } = projectData;
}
