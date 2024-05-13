using Avalonia.Controls;
using Avalonia.Platform;

namespace ObjectDetection.Gui.Common.Services.IconProvider;

/// <summary>Провайдер иконки приложения.</summary>
public class ApplicationIconProvider : IApplicationIconProvider
{
	/// <inheritdoc/>
	public WindowIcon Icon { get; }

	/// <summary>Создаёт экземпляр класса <see cref="ApplicationIconProvider"/>.</summary>
	public ApplicationIconProvider(string iconPath)
	{
		Icon = new WindowIcon(AssetLoader.Open(new Uri(iconPath)));
	}
}
