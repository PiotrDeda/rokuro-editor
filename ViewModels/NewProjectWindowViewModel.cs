using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace RokuroEditor.ViewModels;

public class NewProjectWindowViewModel : ViewModelBase
{
	string _projectName = "";
	string _projectPath = "";
	string _resultingPath = "";

	public NewProjectWindowViewModel()
	{
		CreateCommand = ReactiveCommand.Create(() => ResultingPath)!;
		CancelCommand = ReactiveCommand.Create(() => (string?)null);
	}

	public string ProjectPath
	{
		get => _projectPath;
		set
		{
			_projectPath = this.RaiseAndSetIfChanged(ref _projectPath, value);
			UpdateResultingPath();
		}
	}

	public string ProjectName
	{
		get => _projectName;
		set
		{
			_projectName = value;
			UpdateResultingPath();
		}
	}

	public string ResultingPath
	{
		get => _resultingPath;
		private set => this.RaiseAndSetIfChanged(ref _resultingPath, value);
	}

	public Interaction<string, string?> SelectProjectPath { get; } = new();
	public ReactiveCommand<Unit, string?> CreateCommand { get; }
	public ReactiveCommand<Unit, string?> CancelCommand { get; }

	void UpdateResultingPath() => ResultingPath = Path.Combine(ProjectPath, ProjectName, $"{ProjectName}.csproj");

	public async void SelectProjectPathCommand()
	{
		string? projectPath = await SelectProjectPath.Handle("Select Folder");
		if (projectPath != null)
			ProjectPath = projectPath;
	}
}
