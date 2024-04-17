using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using RokuroEditor.ViewModels;

namespace RokuroEditor.Views.Panels;

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
	string CannotReloadWhileRunningMessage => "Cannot reload while running";

	[SuppressMessage("ReSharper", "UnusedParameter.Local")]
	async void RunButton_OnClick(object? sender, RoutedEventArgs e)
	{
		if (IsRunning)
			return;
		IsRunning = true;

		ViewModel!.ProjectData.SaveProject();
		if (!ViewModel!.ProjectData.BuildProject())
		{
			IsRunning = false;
			ViewModel!.FlyoutMessage = NoProjectOpenMessage;
			FlyoutBase.ShowAttachedFlyout((sender as Control)!);
			return;
		}

		try
		{
			await ProjectBuilder.Run(RunProcess, ViewModel.ProjectData.ProjectName!, ViewModel.ProjectData.Log)
				.WaitForExitAsync();
		}
		catch (Exception exception)
		{
			IsRunning = false;
			ViewModel!.ProjectData.Log(exception.ToString());
		}
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
		if (ViewModel!.ProjectData.ProjectPath == null || ViewModel!.ProjectData.ProjectName == null)
		{
			ViewModel!.FlyoutMessage = NoProjectOpenMessage;
			FlyoutBase.ShowAttachedFlyout((sender as Control)!);
		}

		if (IsRunning)
		{
			ViewModel!.FlyoutMessage = CannotReloadWhileRunningMessage;
			FlyoutBase.ShowAttachedFlyout((sender as Control)!);
			return;
		}

		ViewModel!.ProjectData.SaveProject();
		ViewModel!.ProjectData.BuildProject();
		ViewModel!.ProjectData.ClearLoadedData();
		ViewModel!.ProjectData.LoadProject();
	}
}
