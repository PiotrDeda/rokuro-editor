using Oirenomi.Models;

namespace Oirenomi.ViewModels;

public class ListPanelViewModel(ProjectData projectData) : ViewModelBase
{
	public ListPanelViewModel() : this(new())
	{
		ProjectData.LoadSampleData();
	}

	public ProjectData ProjectData { get; set; } = projectData;
}
