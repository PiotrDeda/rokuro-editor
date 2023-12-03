using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Oirenomi.ViewModels;

namespace Oirenomi.Views.Panels;

public partial class RunButtonsPanel : ReactiveUserControl<MenuPanelViewModel>
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

	[SuppressMessage("ReSharper", "UnusedParameter.Local")]
	async void RunButton_OnClick(object? sender, RoutedEventArgs e)
	{
		if (IsRunning)
			return;
		if (ViewModel!.ProjectPath is null || ViewModel!.ProjectName is null)
		{
			FlyoutBase.ShowAttachedFlyout(this);
			return;
		}
		string projectPath = ViewModel.ProjectPath;
		string projectName = ViewModel.ProjectName;

		IsRunning = true;

		using (var buildProcess = new Process())
		{
			buildProcess.StartInfo.FileName = "cmd.exe";
			buildProcess.StartInfo.Arguments = $"/C dotnet build \"{projectPath}\" --output build/{projectName}";
			buildProcess.StartInfo.CreateNoWindow = true;
			buildProcess.StartInfo.UseShellExecute = false;
			buildProcess.StartInfo.RedirectStandardOutput = true;

			buildProcess.Start();
			Console.Write(buildProcess.StandardOutput.ReadToEnd());
			buildProcess.WaitForExit();
		}

		RunProcess.StartInfo.FileName = $"build/{projectName}/{projectName}.exe";
		RunProcess.StartInfo.WorkingDirectory = $"build/{projectName}";
		RunProcess.Start();
		await RunProcess.WaitForExitAsync();
	}

	[SuppressMessage("ReSharper", "UnusedParameter.Local")]
	void StopButton_OnClick(object? sender, RoutedEventArgs e)
	{
		if (IsRunning)
			RunProcess.Kill();
	}
}
