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

/// <summary>������ ������������� ��������������.</summary>
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

	/// <summary>��������� ���������.</summary>
	public ICommand StartCommand { get; }

	/// <summary>�������� ���������.</summary>
	public ICommand StopCommand { get; }

	/// <summary>�������� ���������.</summary>
	public ICommand SettingsCommand { get; }

	#endregion

	#region Properties

	/// <inheritdoc/>
	public ViewModelActivator Activator { get; }

	/// <summary>������������ �����������.</summary>
	public Bitmap? Picture
	{
		get => _picture;
		set
		{
			_picture?.Dispose();
			this.RaiseAndSetIfChanged(ref _picture, value);
		}
	}

	/// <summary>������ ���������.</summary>
	public bool IsRunningProcessing
	{
		get => _isRunningProcessing;
		set => this.RaiseAndSetIfChanged(ref _isRunningProcessing, value);
	}

	/// <summary>�������������� ��� �������� ������.</summary>
	public Interaction<Unit, string?> ShowFileDialogWindow { get; set; } = new();

	/// <summary>�������������� � ����� ��������.</summary>
	public Interaction<FFmpegImageSourceSettingWindowViewModel, Unit?> ShowSettingsWindow { get; set; } = new();

	/// <summary>�������������� � ����������� ������.</summary>
	public DialogServiceInteractions DialogServiceInteractions { get; init; } = new();

	/// <summary>������� ��������� ����������� ���������.</summary>
	public event EventHandler<List<ObjectInfo>> DetectionResultsEvent = delegate { };

	#endregion

	#region .ctor

	/// <summary>������ ��������� ������ <see cref="VideoProcessingViewModel"/>.</summary>
	/// <param name="config">������������ ����������.</param>
	/// <param name="processingParameters">��������� ���������.</param>
	/// <param name="detectorWorker">��������.</param>
	/// <param name="videoFrameExtractor">����������� ����� �� �����.</param>
	/// <param name="imageConverter">��������� �����������.</param>
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

	/// <summary>������ �����������.</summary>
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

	/// <summary>��������� �����������.</summary>
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

	/// <summary>�������� ���� ��������.</summary>
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
