using System.Diagnostics;
using System.Reactive.Linq;
using Oirenomi.Models;
using ReactiveUI;

namespace Oirenomi.ViewModels;

public class MenuPanelViewModel(ProjectData projectData) : ViewModelBase
{
	public Interaction<string, string?> SelectProjectPath { get; } = new();
	public ProjectData ProjectData { get; } = projectData;

	public async void OpenProjectCommand() =>
		ProjectData.SetProjectPathAndName(await SelectProjectPath.Handle("Open Project"));

	public void ExitCommand() => Process.GetCurrentProcess().CloseMainWindow();
}
