using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using RokuroEditor.Models;

namespace RokuroEditor.ViewModels;

public class MenuPanelViewModel(ProjectData projectData) : ViewModelBase
{
	public MenuPanelViewModel() : this(new())
	{
		ProjectData.LoadSampleData();
	}

	public Interaction<Unit, string?> OpenNewProjectMenu { get; } = new();
	public Interaction<string, string?> SelectProjectPath { get; } = new();
	public Interaction<SettingsWindowViewModel, Unit> OpenSettingsMenu { get; } = new();
	public ProjectData ProjectData { get; } = projectData;

	public async void NewProjectCommand()
	{
		string? projectPath = await OpenNewProjectMenu.Handle(Unit.Default);
		if (projectPath is not null)
			try
			{
				ProjectData.SetProjectPathAndName(projectPath);
				if (!Directory.Exists(ProjectData.ProjectPath))
					Directory.CreateDirectory(ProjectData.ProjectPath!);
				ProjectData.NewProject();
			}
			catch (Exception e)
			{
				Console.WriteLine("Could not create project:");
				Console.WriteLine(e);
			}
		ProjectData.LoadProject();
	}

	public async void OpenProjectCommand()
	{
		ProjectData.SetProjectPathAndName(await SelectProjectPath.Handle("Open Project"));
		ProjectData.LoadProject();
	}

	public void SaveProjectCommand() => ProjectData.SaveProject();

	public async void SettingsCommand() => await OpenSettingsMenu.Handle(new(ProjectData));

	public void ExitCommand() => Process.GetCurrentProcess().CloseMainWindow();
}
