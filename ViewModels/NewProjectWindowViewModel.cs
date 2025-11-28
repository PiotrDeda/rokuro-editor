using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace RokuroEditor.ViewModels;

public class NewProjectWindowViewModel : ViewModelBase
{
	public NewProjectWindowViewModel()
	{
		CreateCommand = ReactiveCommand.Create(() => ResultingPath)!;
		CancelCommand = ReactiveCommand.Create(string? () => null);
	}

	public string ProjectPath
	{
		get;
		set
		{
			field = this.RaiseAndSetIfChanged(ref field, value);
			UpdateResultingPath();
		}
	} = "";

	public string ProjectName
	{
		get;
		set
		{
			field = value;
			UpdateResultingPath();
		}
	} = "";

	public string ResultingPath
	{
		get;
		private set => this.RaiseAndSetIfChanged(ref field, value);
	} = "";

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
