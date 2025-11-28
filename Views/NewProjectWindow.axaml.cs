using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.ReactiveUI;
using ReactiveUI;
using RokuroEditor.ViewModels;

namespace RokuroEditor.Views;

public partial class NewProjectWindow : ReactiveWindow<NewProjectWindowViewModel>
{
	public NewProjectWindow()
	{
		InitializeComponent();

		this.WhenActivated(() => [
			ViewModel!.CreateCommand.Subscribe(_ => Close()),
			ViewModel!.CancelCommand.Subscribe(_ => Close()),
			ViewModel!.SelectProjectPath.RegisterHandler(SelectProjectPathHandler)
		]);
	}

	async Task SelectProjectPathHandler(IInteractionContext<string, string?> context)
	{
		try
		{
			var storageFiles = await GetTopLevel(this)!.StorageProvider.OpenFolderPickerAsync(new() {
				Title = context.Input
			});

			if (storageFiles.Any())
				context.SetOutput(storageFiles.First().Path.LocalPath);
			else
				context.SetOutput(null);
		}
		catch
		{
			context.SetOutput(null);
		}
	}
}
