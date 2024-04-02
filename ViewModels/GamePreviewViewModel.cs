using RokuroEditor.Models;

namespace RokuroEditor.ViewModels;

public class GamePreviewViewModel(ProjectData projectData) : ViewModelBase
{
	public GamePreviewViewModel() : this(new())
	{
		ProjectData.LoadSampleData();
	}

	public ProjectData ProjectData { get; set; } = projectData;
}
