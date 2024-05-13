using System.Collections.Generic;
using System.Reactive.Disposables;

using Avalonia.Collections;

using ReactiveUI;

using ObjectDetection.Gui.Common.ViewModels;
using ObjectDetection.Processing.Helpers;

namespace ObjectDetection.Gui.ViewModels;

/// <summary>Модель представления таблицы с результатами обработки.</summary>
public class DetectionTableViewModel : ViewModelBase, IActivatableViewModel
{
	private AvaloniaList<ObjectInfo> _detectionResults = [];

	#region Properties

	/// <inheritdoc/>
	public ViewModelActivator Activator { get; }

	/// <summary>Результативные данные работы детектора.</summary>
	public AvaloniaList<ObjectInfo> DetectionResults
	{
		get => _detectionResults;
		set => this.RaiseAndSetIfChanged(ref _detectionResults, value);
	}

	#endregion

	/// <summary>Создаёт экземпляр класса <see cref="DetectionTableViewModel"/>.</summary>
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

	/// <summary>Добавление информации в таблицу.</summary>
	/// <param name="detectionResults">Результаты обработки.</param>
	public void AddingInformationToTable(List<ObjectInfo> detectionResults) => DetectionResults = new(detectionResults);
}
