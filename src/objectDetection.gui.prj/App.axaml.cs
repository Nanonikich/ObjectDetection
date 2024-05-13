using System;

using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML.OnnxRuntime;

using ReactiveUI;

using Splat.Microsoft.Extensions.DependencyInjection;
using Splat;

using ObjectDetection.Configuration;
using ObjectDetection.Gui.Common.Services.IconProvider;
using ObjectDetection.Gui.Helpers;
using ObjectDetection.Gui.ViewModels;
using ObjectDetection.Gui.Views.Windows;
using ObjectDetection.Processing.Converters;
using ObjectDetection.Processing;

namespace ObjectDetection.Gui;

public partial class App : Application
{
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public IServiceProvider Container { get; private set; }

	public App()
	{
		var host = Host
			.CreateDefaultBuilder()
			.ConfigureServices(service =>
			{
				service.UseMicrosoftDependencyResolver();
				service
					.AddSingleton(provider => 
					{
						var configuration = new ProcessConfiguration().Load();
						return new InferenceSession(configuration.Networks.Detector.DetectorPath);
					})
					.AddSingleton<RunOptions>()
					.AddSingleton<DetectorWorker>()
					.AddSingleton<VideoFrameExtractor>()
					.AddSingleton<IImageConverter, ImageConverter>()
					.AddSingleton<IApplicationIconProvider>(provider => new ApplicationIconProvider($"avares://ObjectDetection.Gui.Common/Assets/avalonia-logo.ico"))
					.AddTransient<ProcessingParameters>()
					.AddTransient<VideoProcessingViewModel>()
					.AddTransient<DetectionTableViewModel>()
					.AddTransient<FFmpegImageSourceSettingWindowViewModel>();

				var resolver = Locator.CurrentMutable;
				resolver.InitializeSplat();
				resolver.InitializeReactiveUI();

				RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
				RxApp.DefaultExceptionHandler = new ExceptionHandler();
			})
			.Build();

		Container = host.Services;
		Container.UseMicrosoftDependencyResolver();
		host.Start();
	}

	public override void OnFrameworkInitializationCompleted()
	{
		if(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			if(CheckActivation())
			{
				desktop.MainWindow = new MainWindow
				{
					DataContext = new MainWindowViewModel()
					{
						VideoProcessingViewModel = Container.GetRequiredService<VideoProcessingViewModel>(),
						DetectionTableViewModel = Container.GetRequiredService<DetectionTableViewModel>(),
					}
				};
			}
		}

		base.OnFrameworkInitializationCompleted();
	}

	private bool CheckActivation() => true;

}

class ExceptionHandler : IObserver<Exception>
{
	public void OnCompleted()
	{

	}

	public void OnError(Exception error)
	{

	}

	public void OnNext(Exception value)
	{
		throw new NotImplementedException();
	}
}
