using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using RokuroEditor.Models;
using RokuroEditor.Views.Controls;

namespace RokuroEditor.ViewModels;

public class PropertiesPanelViewModel : ViewModelBase
{
	public PropertiesPanelViewModel() : this(new())
	{
		ProjectData.LoadSampleData();
		ProjectData.SelectedScene = ProjectData.Scenes[1];
		ProjectData.SelectedGameObject = ProjectData.SelectedScene.GameObjects[0];
		ProjectData.SelectedCamera = ProjectData.SelectedScene.Cameras[0];
	}

	public PropertiesPanelViewModel(ProjectData projectData)
	{
		ProjectData = projectData;
		DeleteSceneCommand = ReactiveCommand.CreateFromTask(() =>
			ShowConfirmationDialog(ProjectData.DeleteSelectedScene, ProjectData.SelectedScene));
		DeleteGameObjectCommand = ReactiveCommand.CreateFromTask(() =>
			ShowConfirmationDialog(ProjectData.DeleteSelectedGameObject, ProjectData.SelectedGameObject));
		DeleteCameraCommand = ReactiveCommand.CreateFromTask(() =>
			ShowConfirmationDialog(ProjectData.DeleteSelectedCamera, ProjectData.SelectedCamera));
	}

	public ProjectData ProjectData { get; }

	public ReactiveCommand<Unit, Unit>? DeleteSceneCommand { get; }
	public ReactiveCommand<Unit, Unit>? DeleteGameObjectCommand { get; }
	public ReactiveCommand<Unit, Unit>? DeleteCameraCommand { get; }

	async Task ShowConfirmationDialog(Action onConfirm, object? objectCheck)
	{
		if (objectCheck == null)
			return;
		Window mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow!;
		var dialog = new Window {
			Width = 250, Height = 100, CanResize = false, ShowInTaskbar = false,
			WindowStartupLocation = WindowStartupLocation.CenterOwner
		};
		dialog.Content = new ConfirmDialog {
			DataContext = new ConfirmDialogViewModel(() => {
				onConfirm();
				dialog.Close();
			}, () => {
				dialog.Close();
			})
		};
		await dialog.ShowDialog(mainWindow);
	}
}
