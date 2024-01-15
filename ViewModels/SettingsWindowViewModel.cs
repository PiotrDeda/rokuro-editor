using RokuroEditor.Models;

namespace RokuroEditor.ViewModels;

public class SettingsWindowViewModel(ProjectData projectData) : ViewModelBase
{
	public SettingsWindowViewModel() : this(new())
	{
		ProjectData.LoadSampleData();
	}

	public ProjectData ProjectData { get; } = projectData;
}
