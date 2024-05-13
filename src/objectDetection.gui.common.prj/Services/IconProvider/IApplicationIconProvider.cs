using Avalonia.Controls;

namespace ObjectDetection.Gui.Common.Services.IconProvider;

/// <summary>Иконка для окон.</summary>
public interface IApplicationIconProvider
{
	/// <summary>Иконка.</summary>
	WindowIcon Icon { get; }
}
