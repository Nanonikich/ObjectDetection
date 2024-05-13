using System.Reactive;

using Avalonia.Platform.Storage;

using ReactiveUI;

namespace ObjectDetection.Gui.Common.Services.Dialogs;

/// <summary>Источник взаимодействий, связанных с проводником.</summary>
public class FileDialogInteractionSource
{
	private readonly IStorageProvider _storageProvider;

	public FileDialogInteractionSource(IStorageProvider storageProvider)
	{
		_storageProvider = storageProvider;
	}

	/// <summary>Открытие окна для выбора файла.</summary>
	/// <param name="interaction">Взаимодействие открытия выбора файла.</param>
	/// <returns></returns>
	public async Task DoShowDialog(InteractionContext<Unit, string?> interaction)
	{
		var dialog = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
		{
			AllowMultiple = false,
			Title = "Open Video File",
			FileTypeFilter = new List<FilePickerFileType>
			{
				new("Video Files ( *.mp4 *.avi *.ts *.mkv *.mov *.wmv *.m4v )")
				{
					Patterns = new List<string> { "*.mp4", "*.avi", "*.ts", "*.mkv", "*.mov", "*.wmv", "*.m4v" },
				},
				new("Image Files ( *.png *.jpg *.bmp *.gif )")
				{
					Patterns = new List<string> { "*.png", "*.jpg", "*.bmp", "*.gif" },
				},
				new("All Files")
				{
					Patterns = new List<string> { "*.*" },
				},
			},
		});

		if(dialog.Any())
		{
			interaction.SetOutput(dialog[0].Path.LocalPath);
		}
		else
		{
			interaction.SetOutput(default);
		}
	}
}
