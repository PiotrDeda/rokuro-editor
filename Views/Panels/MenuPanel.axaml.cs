using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using Oirenomi.ViewModels;
using ReactiveUI;

namespace Oirenomi.Views.Panels;

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

		context.SetOutput(storageFiles.First().Path.LocalPath);
		Console.WriteLine(storageFiles.First().Path.LocalPath);
	}
}
