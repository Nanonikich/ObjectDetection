using System.Reactive.Disposables;
using System.Collections.Generic;
using System;

using Avalonia.ReactiveUI;

using ReactiveUI;

using ObjectDetection.Gui.Common.Services.Dialogs;
using ObjectDetection.Gui.ViewModels;

namespace ObjectDetection.Gui.Views.Windows;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
	private DialogReactiveWindow _dialogReactiveWindow;
	private List<IDisposable> _interactionsDispose;


	public MainWindow()
	{
		InitializeComponent();

		_dialogReactiveWindow = new(this);

		this.WhenActivated(_ =>
		{
			var iteractionImpl = new DialogServiceInteractionSource<MainWindow>(this);
			iteractionImpl.Register(ViewModel!.VideoProcessingViewModel.DialogServiceInteractions);

			_interactionsDispose = new();

			_interactionsDispose.AddRange(new List<IDisposable>()
			{
				ViewModel!.VideoProcessingViewModel.ShowFileDialogWindow
						.RegisterHandler(new FileDialogInteractionSource(this.StorageProvider).DoShowDialog),

				ViewModel!.VideoProcessingViewModel.ShowSettingsWindow
					.RegisterHandler(_dialogReactiveWindow.ShowDialogWindow<FFmpegImageSourceSettingWindowViewModel, FFmpegImageSourceSettingWindow>),
			});

			Disposable
				.Create(() =>
				{
					iteractionImpl.Dispose();
					_interactionsDispose.ForEach(x => x.Dispose());
					_interactionsDispose.Clear();
				})
				.DisposeWith(_);
		});

	}
}
