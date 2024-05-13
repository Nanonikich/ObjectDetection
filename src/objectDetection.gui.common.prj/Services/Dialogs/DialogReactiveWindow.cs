using System.Reactive;

using Avalonia.Controls;

using MsBox.Avalonia.Enums;

using ReactiveUI;

using ObjectDetection.Gui.Common.Helpers;
using ObjectDetection.Gui.Common.Services.IconProvider;
using ObjectDetection.Gui.Common.ViewModels;

namespace ObjectDetection.Gui.Common.Services.Dialogs;

/// <summary>Обработчик показа сообщений.</summary>
public class DialogReactiveWindow
{
	private readonly Window? _window;

	public IApplicationIconProvider IconProvider { get; set; }

	/// <summary>Создаёт экземпляр класса <see cref="DialogReactiveWindow"/>.</summary>
	/// <param name="window">Окно-родитель.</param>
	public DialogReactiveWindow(Window window)
	{
		_window = window;
	}

	/// <summary>Показывает диалоговое окно.</summary>
	/// <typeparam name="TViewModel">Модель представления вызываемого окна.</typeparam>
	/// <typeparam name="TDialogWindow">Окно модели представления.</typeparam>
	/// <param name="interaction">Связь для вызова.</param>
	/// <returns></returns>
	public async Task ShowDialogWindow<TViewModel, TDialogWindow>(InteractionContext<TViewModel, Unit?> interaction)
		where TViewModel : ViewModelBase
		where TDialogWindow : Window, new()
	{
		var dialog = new TDialogWindow
		{
			DataContext = interaction.Input
		};

		var result = await dialog.ShowDialog<Unit?>(_window);
		interaction.SetOutput(result);
	}

	/// <summary>Показывает сообщение с описанием ошибки.</summary>
	/// <param name="interaction">Связь для вызова.</param>
	/// <returns></returns>
	public async Task ShowErrorMessage(InteractionContext<MessageContent, bool> interaction)
	{
		var msBoxStandardWindow = MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard(
			new MsBox.Avalonia.Dto.MessageBoxStandardParams
			{
				ButtonDefinitions = ButtonEnum.Ok,
				Icon = Icon.Error,
				ContentTitle = interaction.Input.Title,
				ContentMessage = interaction.Input.Message,
				WindowStartupLocation = WindowStartupLocation.CenterScreen,
				WindowIcon = IconProvider.Icon,
			});

		interaction.SetOutput(await msBoxStandardWindow.ShowWindowDialogAsync(_window) == ButtonResult.Ok);
	}

	/// <summary>Показывает сообщение с вопросом.</summary>
	/// <param name="interaction">Связь для вызова.</param>
	/// <returns></returns>
	public async Task ShowQuestionMessage(InteractionContext<MessageContent, bool> interaction)
	{
		var msBoxStandardWindow = MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard(
			new MsBox.Avalonia.Dto.MessageBoxStandardParams
			{
				ButtonDefinitions = ButtonEnum.YesNo,
				Icon = Icon.Question,
				ContentTitle = interaction.Input.Title,
				ContentMessage = interaction.Input.Message,
				WindowStartupLocation = WindowStartupLocation.CenterScreen,
				WindowIcon = IconProvider.Icon,
			});

		interaction.SetOutput(await msBoxStandardWindow.ShowWindowDialogAsync(_window) == ButtonResult.Yes);
	}

	/// <summary>Закрытие окна.</summary>
	/// <param name="interaction">Установка вывода.</param>
	/// <returns></returns>
	public async Task CloseWindow(InteractionContext<bool, bool> interaction)
	{
		_window?.Close();
		interaction.SetOutput(true);
	}
}
