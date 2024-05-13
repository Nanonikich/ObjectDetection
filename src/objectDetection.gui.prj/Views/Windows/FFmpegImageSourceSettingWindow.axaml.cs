using System.Linq;
using System.Reactive.Disposables;

using Avalonia.ReactiveUI;
using Avalonia.Controls;
using Avalonia.Input;

using ReactiveUI;

using ObjectDetection.Gui.Common.Services.Dialogs;
using ObjectDetection.Gui.ViewModels;

namespace ObjectDetection.Gui.Views.Windows;

public partial class FFmpegImageSourceSettingWindow : ReactiveWindow<FFmpegImageSourceSettingWindowViewModel>
{
	private FFmpegImageSourceSettingWindowViewModel _viewModel;

	public FFmpegImageSourceSettingWindow()
	{
		InitializeComponent();

		this.WhenActivated(_ =>
		{
			var iteractionImpl = new DialogServiceInteractionSource<FFmpegImageSourceSettingWindow>(this);
			iteractionImpl.Register(ViewModel!.DialogServiceInteractions);

			_viewModel = ViewModel;

			Disposable
				.Create(() => { iteractionImpl.Dispose(); })
				.DisposeWith(_);
		});

		SetupDnd();
	}

	/// <summary>Установка drag'n'drop для окна.</summary>
	private void SetupDnd()
	{
		void DragOver(object? sender, DragEventArgs e)
		{
			if(e.Source is Control c)
				e.DragEffects &= (DragDropEffects.Move);

			if(!e.Data.Contains(DataFormats.FileNames))
				e.DragEffects = DragDropEffects.None;
		}

		void Drop(object? sender, DragEventArgs e)
		{
			if(e.Source is Control c)
				e.DragEffects &= (DragDropEffects.Move);

			if(e.Data.Contains(DataFormats.FileNames))
				_viewModel.UriVideoStream = e.Data.GetFiles()
					.Select(x => x.Name)
					.FirstOrDefault();
		}

		AddHandler(DragDrop.DropEvent, Drop);
		AddHandler(DragDrop.DragOverEvent, DragOver);
	}
}
