using System.Diagnostics;
using System.Reactive.Linq;
using RokuroEditor.Models;
using ReactiveUI;

namespace RokuroEditor.ViewModels;

public class MenuPanelViewModel(ProjectData projectData) : ViewModelBase
{
	public MenuPanelViewModel() : this(new())
	{
		ProjectData.LoadSampleData();
	}

	public Interaction<string, string?> SelectProjectPath { get; } = new();
	public ProjectData ProjectData { get; } = projectData;

	public async void OpenProjectCommand() =>
		ProjectData.SetProjectPathAndName(await SelectProjectPath.Handle("Open Project"));

	public void ExitCommand() => Process.GetCurrentProcess().CloseMainWindow();
}
