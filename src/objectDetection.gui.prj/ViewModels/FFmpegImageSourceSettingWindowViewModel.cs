using System.Reactive.Linq;
using System.Reactive;
using System.Windows.Input;
using System;

using ReactiveUI;

using ObjectDetection.Gui.Common.Services.Dialogs;
using ObjectDetection.Gui.Common.ViewModels;
using ObjectDetection.Gui.Helpers;

namespace ObjectDetection.Gui.ViewModels;

/// <summary>Модель представления настроек видеоисточника.</summary>
public class FFmpegImageSourceSettingWindowViewModel : ViewModelBase
{
	private readonly Interaction<Unit, string?> _fileDialogInteraction;
	private string _uriVideoStream;

	/// <summary>Событие применения настроек видеоисточника.</summary>
	public event EventHandler<bool> ApplyClicked;

	/// <summary>Uri-путь к источнику обработки.</summary>
	public string UriVideoStream
	{
		get => _uriVideoStream;
		set => this.RaiseAndSetIfChanged(ref _uriVideoStream, value);
	}

	public DialogServiceInteractions DialogServiceInteractions { get; init; } = new();

	#region Commands

	/// <summary>Выбрать источник для дальнейшей обработки.</summary>
	public ICommand SelectVideoFileCommand { get; }

	/// <summary>Сохранить изменения.</summary>
	public ICommand ApplyCommand { get; }

	/// <summary>Отменить изменения.</summary>
	public ICommand CancelCommand { get; }

	#endregion

	#region .ctor

	/// <summary>Создаёт экземпляр класса <see cref="FFmpegImageSourceSettingWindowViewModel"/>.</summary>
	/// <param name="processingParameters">Параметры обработки.</param>
	/// <param name="fileDialogInteraction">Окно выбора видеоисточника.</param>
	public FFmpegImageSourceSettingWindowViewModel(ProcessingParameters processingParameters, 
		Interaction<Unit, string?> fileDialogInteraction)
	{
		_fileDialogInteraction = fileDialogInteraction;

		GetStartVideoParameters(processingParameters);

		SelectVideoFileCommand = ReactiveCommand.CreateFromTask(async () =>
		{
			if(await _fileDialogInteraction.Handle(Unit.Default) is { } path)
				UriVideoStream = path;
		});

		ApplyCommand = ReactiveCommand.CreateFromTask(async () =>
		{
			if(UriVideoStream != default)
			{
				SaveSetup(processingParameters);
				ApplyClicked?.Invoke(this, true);
			}
			await DialogServiceInteractions.CloseWindow.Handle(true);
		});

		CancelCommand = ReactiveCommand.CreateFromTask(async () => await DialogServiceInteractions.CloseWindow.Handle(true));
	}

	#endregion

	/// <summary>Получение настроек видеоисточника по умолчанию.</summary>
	/// <param name="videoData">Данные видеоисточника.</param>
	private void GetStartVideoParameters(ProcessingParameters videoData)
	{
		if(videoData.Url != default)
			UriVideoStream = videoData.Url;
	}

	/// <summary>Сохранение настроек для видеоисточника.</summary>
	/// <param name="videoData">Данные видеоисточника.</param>
	private void SaveSetup(ProcessingParameters videoData)
		=> videoData.Url = UriVideoStream;
}
