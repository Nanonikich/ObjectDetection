using Avalonia.ReactiveUI;
using ObjectDetection.Gui.ViewModels;

namespace ObjectDetection.Gui.Views.Controls;

public partial class VideoProcessingView : ReactiveUserControl<VideoProcessingViewModel>
{
	public VideoProcessingView()
	{
		InitializeComponent();
	}
}
