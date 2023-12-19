using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
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

		this.WhenActivated(d => { d(ViewModel!.SelectProjectPath.RegisterHandler(SelectProjectPathHandler)); });
	}

	async Task SelectProjectPathHandler(InteractionContext<string, string?> context)
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
}
