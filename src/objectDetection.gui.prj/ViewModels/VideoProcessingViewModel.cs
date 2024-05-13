using System.Collections.Generic;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Input;
using System;

using Avalonia.Media.Imaging;

using ReactiveUI;

using ObjectDetection.Configuration;
using ObjectDetection.Gui.Common.Helpers;
using ObjectDetection.Gui.Common.Services.Dialogs;
using ObjectDetection.Gui.Common.ViewModels;
using ObjectDetection.Gui.Helpers;
using ObjectDetection.Processing.Converters;
using ObjectDetection.Processing.Helpers;
using ObjectDetection.Processing;

namespace ObjectDetection.Gui.ViewModels;

/// <summary>Модель представления видеообработки.</summary>
public class VideoProcessingViewModel : ViewModelBase, IActivatableViewModel
{
	#region Fields

	private readonly Networks _networks;
	private readonly DetectorWorker _detectorWorker;
	private readonly VideoFrameExtractor _videoFrameExtractor;
	private readonly ProcessingParameters _processingParameters;
	private readonly IImageConverter _imageConverter;
	private CancellationTokenSource _tokenSource = new();
	private Bitmap? _picture;
	private bool _isRunningProcessing;

	#endregion

	#region Commands

	/// <summary>Сохранить изменения.</summary>
	public ICommand StartCommand { get; }

	/// <summary>Отменить изменения.</summary>
	public ICommand StopCommand { get; }

	/// <summary>Отменить изменения.</summary>
	public ICommand SettingsCommand { get; }

	#endregion

	#region Properties

	/// <inheritdoc/>
	public ViewModelActivator Activator { get; }

	/// <summary>Показываемое изображение.</summary>
	public Bitmap? Picture
	{
		get => _picture;
		set
		{
			_picture?.Dispose();
			this.RaiseAndSetIfChanged(ref _picture, value);
		}
	}

	/// <summary>Статус обработки.</summary>
	public bool IsRunningProcessing
	{
		get => _isRunningProcessing;
		set => this.RaiseAndSetIfChanged(ref _isRunningProcessing, value);
	}

	/// <summary>Взаимодействие для открытия файлов.</summary>
	public Interaction<Unit, string?> ShowFileDialogWindow { get; set; } = new();

	/// <summary>Взаимодействие с окном настроек.</summary>
	public Interaction<FFmpegImageSourceSettingWindowViewModel, Unit?> ShowSettingsWindow { get; set; } = new();

	/// <summary>Взаимодействие с диалоговыми окнами.</summary>
	public DialogServiceInteractions DialogServiceInteractions { get; init; } = new();

	/// <summary>Событие получение результатов обработки.</summary>
	public event EventHandler<List<ObjectInfo>> DetectionResultsEvent = delegate { };

	#endregion

	#region .ctor

	/// <summary>Создаёт экземпляр класса <see cref="VideoProcessingViewModel"/>.</summary>
	/// <param name="config">Конфигурация приложения.</param>
	/// <param name="processingParameters">Параметры обработки.</param>
	/// <param name="detectorWorker">Детектор.</param>
	/// <param name="videoFrameExtractor">Сегментатор видео на кадры.</param>
	/// <param name="imageConverter">Конвертер изображений.</param>
	public VideoProcessingViewModel(
		ApplicationConfiguration config, 
		ProcessingParameters processingParameters,
		DetectorWorker detectorWorker, 
		VideoFrameExtractor videoFrameExtractor,
		IImageConverter imageConverter)
	{
		Activator = new();
		this.WhenActivated((disposables) =>
		{
			Disposable
				.Create(() =>
				{
					StopPlayer();
				})
				.DisposeWith(disposables);
		});

		_networks = config.Networks;
		_detectorWorker = detectorWorker;
		_videoFrameExtractor = videoFrameExtractor;
		_imageConverter = imageConverter;
		_processingParameters = processingParameters;

		StartCommand = ReactiveCommand.CreateFromTask(StartPlayer);

		StopCommand = ReactiveCommand.Create(StopPlayer);

		SettingsCommand = ReactiveCommand.CreateFromTask(SettingsPlayer);
	}

	#endregion

	#region Methods

	/// <summary>Запуск видеоплеера.</summary>
	private async Task StartPlayer()
	{
		if(_processingParameters.Url != default)
		{
			IsRunningProcessing = true;

			_videoFrameExtractor.FrameCaptureEvent += OnFrameProcessing;
			_tokenSource = new();
			new Task(() =>
			{
				_videoFrameExtractor.InitializeFrameExtractor(_processingParameters.Url, _tokenSource.Token);
			}, _tokenSource.Token).Start();
		}
		else
		{
			await DialogServiceInteractions.ShowErrorMessage.Handle(new MessageContent
			{
				Title = "Error",
				Message = "Invalid connection string"
			});
		}
	}

	/// <summary>Остановка видеоплеера.</summary>
	private void StopPlayer()
	{
		_tokenSource.Cancel();
		
		_videoFrameExtractor.Dispose();

		_picture?.Dispose();
		Picture = null;

		_videoFrameExtractor.FrameCaptureEvent -= OnFrameProcessing;

		DetectionResultsEvent?.Invoke(this, []);

		IsRunningProcessing = false;
	}

	/// <summary>Открытие окна настроек.</summary>
	private async Task SettingsPlayer()
	{
		var settingsWindow = new FFmpegImageSourceSettingWindowViewModel(_processingParameters, ShowFileDialogWindow);
		await ShowSettingsWindow.Handle(settingsWindow);
	}

	#endregion

	private void OnFrameProcessing(object? sender, byte[] e)
	{
		var processingResults = _detectorWorker.FrameProcessing(_networks.Detector.DetectorPath, e);

		if(!_tokenSource.Token.IsCancellationRequested)
		{
			using MemoryStream ms = new(_imageConverter.SixLaborsImageToArray(processingResults.Picture));
			Picture = new Bitmap(ms);

			processingResults.Dispose();

			DetectionResultsEvent?.Invoke(this, processingResults.DetectionResults);
		}
	}
}
