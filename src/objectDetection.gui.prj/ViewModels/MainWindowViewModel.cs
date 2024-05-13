using System.Collections.Generic;
using System.Reactive.Disposables;

using ReactiveUI;

using ObjectDetection.Gui.Common.ViewModels;
using ObjectDetection.Processing.Helpers;

namespace ObjectDetection.Gui.ViewModels;

/// <summary>Модель представления главного окна.</summary>
public class MainWindowViewModel : ViewModelBase, IActivatableViewModel
{
	private VideoProcessingViewModel _videoProcessingViewModel;
	private DetectionTableViewModel _detectionTableViewModel;

	/// <inheritdoc/>
	public ViewModelActivator Activator { get; }

	/// <summary>Виджет визуализации результатов обработки.</summary>
	public VideoProcessingViewModel VideoProcessingViewModel
	{
		get => _videoProcessingViewModel;
		set => this.RaiseAndSetIfChanged(ref _videoProcessingViewModel, value);
	}

	/// <summary>Таблица, содержащая результаты детектора.</summary>
	public DetectionTableViewModel DetectionTableViewModel
	{
		get => _detectionTableViewModel;
		set => this.RaiseAndSetIfChanged(ref _detectionTableViewModel, value);
	}

	/// <summary>Создаёт экземпляр класса <see cref="MainWindowViewModel"/>.</summary>
	public MainWindowViewModel()
	{
		Activator = new();
		this.WhenActivated((disposables) =>
		{
			VideoProcessingViewModel.DetectionResultsEvent += OnFrameProcessingResults;

			Disposable
				.Create(() =>
				{
					VideoProcessingViewModel.DetectionResultsEvent -= OnFrameProcessingResults;
				})
				.DisposeWith(disposables);
		});
	}

	private void OnFrameProcessingResults(object sender, List<ObjectInfo> detectionResults)
	{
		DetectionTableViewModel.AddingInformationToTable(detectionResults);
	}
}
