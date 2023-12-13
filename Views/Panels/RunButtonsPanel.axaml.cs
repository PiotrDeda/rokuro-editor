using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Oirenomi.ViewModels;

namespace Oirenomi.Views.Panels;

public partial class RunButtonsPanel : ReactiveUserControl<RunButtonsPanelViewModel>
{
	public RunButtonsPanel()
	{
		InitializeComponent();
		RunProcess = new() {
			StartInfo = {
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true
			}
		};
		RunProcess.Exited += (_, _) => IsRunning = false;
	}

	bool IsRunning { get; set; }
	Process RunProcess { get; }
	string NoProjectOpenMessage => "No project is open";
	string CannotReloadMessage => "Cannot reload while running";

	[SuppressMessage("ReSharper", "UnusedParameter.Local")]
	async void RunButton_OnClick(object? sender, RoutedEventArgs e)
	{
		if (IsRunning)
			return;
		IsRunning = true;

		if (!ViewModel!.ProjectData.BuildProject())
		{
			IsRunning = false;
			ViewModel!.FlyoutMessage = NoProjectOpenMessage;
			FlyoutBase.ShowAttachedFlyout((sender as Control)!);
			return;
		}

		await ProjectBuilder.Run(RunProcess, ViewModel.ProjectData.ProjectName!).WaitForExitAsync();
	}

	[SuppressMessage("ReSharper", "UnusedParameter.Local")]
	void StopButton_OnClick(object? sender, RoutedEventArgs e)
	{
		if (IsRunning)
			RunProcess.Kill();
	}

	[SuppressMessage("ReSharper", "UnusedParameter.Local")]
	void ReloadButton_OnClick(object? sender, RoutedEventArgs e)
	{
		if (IsRunning)
		{
			ViewModel!.FlyoutMessage = CannotReloadMessage;
			FlyoutBase.ShowAttachedFlyout((sender as Control)!);
			return;
		}

		if (!ViewModel!.ProjectData.LoadProject())
		{
			ViewModel!.FlyoutMessage = NoProjectOpenMessage;
			FlyoutBase.ShowAttachedFlyout((sender as Control)!);
			return;
		}
	}
}
