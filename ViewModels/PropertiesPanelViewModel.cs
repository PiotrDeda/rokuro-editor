using RokuroEditor.Models;

namespace RokuroEditor.ViewModels;

public class PropertiesPanelViewModel(ProjectData projectData) : ViewModelBase
{
	public PropertiesPanelViewModel() : this(new())
	{
		ProjectData.LoadSampleData();
		ProjectData.SelectedScene = ProjectData.Scenes[1];
		ProjectData.SelectedGameObject = ProjectData.SelectedScene.GameObjects[0];
	}
	
	public ProjectData ProjectData { get; } = projectData;
}
