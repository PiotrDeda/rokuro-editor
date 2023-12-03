using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;

namespace Oirenomi.ViewModels;

public class MenuPanelViewModel : ViewModelBase
{
	public Interaction<string, string?> SelectProjectPath { get; } = new();
	public string? ProjectPath { get; private set; }
	public string? ProjectName { get; private set; }

	public async void OpenProjectCommand()
	{
		ProjectPath = await SelectProjectPath.Handle("Open Project");
		ProjectName = ProjectPath?.Split('\\').Last().Split('.').First();
	}

	public void ExitCommand()
	{
		Process.GetCurrentProcess().CloseMainWindow();
	}
}
