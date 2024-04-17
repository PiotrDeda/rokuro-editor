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
		{
			ProjectData.CloseProject();
			try
			{
				ProjectData.SetProjectPathAndName(projectPath);
				if (!Directory.Exists(ProjectData.ProjectPath))
					Directory.CreateDirectory(ProjectData.ProjectPath!);
			}
			catch (Exception e)
			{
				ProjectData.ConsoleLog += $"Could not create project:\n{e}";
				return;
			}
		}
		if (!ProjectData.NewProject())
			return;
		if (!ProjectData.BuildProject())
			return;
		ProjectData.LoadProject();
	}

	public async void OpenProjectCommand()
	{
		ProjectData.SetProjectPathAndName(await SelectProjectPath.Handle("Open Project"));
		if (!ProjectData.BuildProject())
			return;
		ProjectData.LoadProject();
	}

	public void SaveProjectCommand() => ProjectData.SaveProject();

	public void CloseProjectCommand() => ProjectData.CloseProject();

	public async void SettingsCommand() => await OpenSettingsMenu.Handle(new(ProjectData));

	public void ExitCommand() => Process.GetCurrentProcess().CloseMainWindow();
}
