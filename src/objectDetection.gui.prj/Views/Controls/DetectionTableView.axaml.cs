using Avalonia.ReactiveUI;
using ObjectDetection.Gui.ViewModels;

namespace ObjectDetection.Gui.Views.Controls;

public partial class DetectionTableView : ReactiveUserControl<DetectionTableViewModel>
{
	public DetectionTableView()
	{
		InitializeComponent();
	}
}
