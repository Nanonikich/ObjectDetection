using System.Collections.Generic;
using System.Reactive.Disposables;

using ReactiveUI;

using ObjectDetection.Gui.Common.ViewModels;
using ObjectDetection.Processing.Helpers;

namespace ObjectDetection.Gui.ViewModels;

/// <summary>������ ������������� �������� ����.</summary>
public class MainWindowViewModel : ViewModelBase, IActivatableViewModel
{
	private VideoProcessingViewModel _videoProcessingViewModel;
	private DetectionTableViewModel _detectionTableViewModel;

	/// <inheritdoc/>
	public ViewModelActivator Activator { get; }

	/// <summary>������ ������������ ����������� ���������.</summary>
	public VideoProcessingViewModel VideoProcessingViewModel
	{
		get => _videoProcessingViewModel;
		set => this.RaiseAndSetIfChanged(ref _videoProcessingViewModel, value);
	}

	/// <summary>�������, ���������� ���������� ���������.</summary>
	public DetectionTableViewModel DetectionTableViewModel
	{
		get => _detectionTableViewModel;
		set => this.RaiseAndSetIfChanged(ref _detectionTableViewModel, value);
	}

	/// <summary>������ ��������� ������ <see cref="MainWindowViewModel"/>.</summary>
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
