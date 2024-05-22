using System;
using System.Reactive;
using ReactiveUI;

namespace RokuroEditor.ViewModels;

public class ConfirmDialogViewModel : ReactiveObject
{
	public ConfirmDialogViewModel(Action okAction, Action cancelAction)
	{
		OkCommand = ReactiveCommand.Create(okAction);
		CancelCommand = ReactiveCommand.Create(cancelAction);
	}

	internal ConfirmDialogViewModel()
	{
		OkCommand = ReactiveCommand.Create(() => {});
		CancelCommand = ReactiveCommand.Create(() => {});
	}

	public ReactiveCommand<Unit, Unit> OkCommand { get; }
	public ReactiveCommand<Unit, Unit> CancelCommand { get; }
}
