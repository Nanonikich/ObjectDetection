using System.Collections.Generic;
using System.Reactive.Disposables;

using Avalonia.Collections;

using ReactiveUI;

using ObjectDetection.Gui.Common.ViewModels;
using ObjectDetection.Processing.Helpers;

namespace ObjectDetection.Gui.ViewModels;

/// <summary>������ ������������� ������� � ������������ ���������.</summary>
public class DetectionTableViewModel : ViewModelBase, IActivatableViewModel
{
	private AvaloniaList<ObjectInfo> _detectionResults = [];

	#region Properties

	/// <inheritdoc/>
	public ViewModelActivator Activator { get; }

	/// <summary>�������������� ������ ������ ���������.</summary>
	public AvaloniaList<ObjectInfo> DetectionResults
	{
		get => _detectionResults;
		set => this.RaiseAndSetIfChanged(ref _detectionResults, value);
	}

	#endregion

	/// <summary>������ ��������� ������ <see cref="DetectionTableViewModel"/>.</summary>
	public DetectionTableViewModel()
	{
		Activator = new();
		this.WhenActivated((disposables) =>
		{
			Disposable
				.Create(() => { })
				.DisposeWith(disposables);
		});
	}

	/// <summary>���������� ���������� � �������.</summary>
	/// <param name="detectionResults">���������� ���������.</param>
	public void AddingInformationToTable(List<ObjectInfo> detectionResults) => DetectionResults = new(detectionResults);
}
