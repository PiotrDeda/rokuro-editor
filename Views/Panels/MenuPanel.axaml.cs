using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using ReactiveUI;
using RokuroEditor.ViewModels;

namespace RokuroEditor.Views.Panels;

public partial class MenuPanel : ReactiveUserControl<MenuPanelViewModel>
{
	public MenuPanel()
	{
		InitializeComponent();

		this.WhenActivated(() => [ViewModel!.OpenNewProjectMenu.RegisterHandler(OpenNewProjectMenuHandler)]);
		this.WhenActivated(() => [ViewModel!.SelectProjectPath.RegisterHandler(SelectProjectPathHandler)]);
		this.WhenActivated(() => [ViewModel!.OpenSettingsMenu.RegisterHandler(OpenSettingsMenuHandler)]);
	}

	async Task OpenNewProjectMenuHandler(IInteractionContext<Unit, string?> context)
	{
		var newProjectWindow = new NewProjectWindow();
		newProjectWindow.DataContext = new NewProjectWindowViewModel();
		context.SetOutput(await newProjectWindow.ShowDialog<string?>(
			((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).Windows.First()
		));
	}

	async Task SelectProjectPathHandler(IInteractionContext<string, string?> context)
	{
		var storageFiles = await TopLevel.GetTopLevel(this)!.StorageProvider.OpenFilePickerAsync(new() {
			Title = context.Input,
			FileTypeFilter = new List<FilePickerFileType> {
				new("C# Project File") { Patterns = new List<string> { "*.csproj" } }
			}
		});

		if (storageFiles.Any())
			context.SetOutput(storageFiles.First().Path.LocalPath);
		else
			context.SetOutput(null);
	}

	async Task OpenSettingsMenuHandler(IInteractionContext<SettingsWindowViewModel, Unit> context)
	{
		var settingsWindow = new SettingsWindow();
		settingsWindow.DataContext = context.Input;
		await settingsWindow.ShowDialog(
			((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).Windows.First()
		);
		context.SetOutput(Unit.Default);
	}
}
