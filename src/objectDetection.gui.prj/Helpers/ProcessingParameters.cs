using ObjectDetection.Gui.Common.ViewModels;

namespace ObjectDetection.Gui.Helpers;

/// <summary>Параметры обработки.</summary>
public class ProcessingParameters : ViewModelBase
{
	/// <summary>Путь к выбранному источнику для обработки.</summary>
	public string Url { get; set; }
}
