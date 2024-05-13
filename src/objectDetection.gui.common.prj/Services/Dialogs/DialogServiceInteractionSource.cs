using Avalonia.Controls;
using Splat;
using ObjectDetection.Gui.Common.Services.IconProvider;

namespace ObjectDetection.Gui.Common.Services.Dialogs;

/// <summary>Создаёт взаимодействия открытия для окон.</summary>
/// <typeparam name="TViewModel">Модель представления окна для взаимодействий.</typeparam>
public class DialogServiceInteractionSource<TViewModel> : IDisposable
	where TViewModel : Window
{
	private readonly DialogReactiveWindow _dialogReactiveWindow;
	private List<IDisposable> _disposeInteractions;

	/// <summary>Создаёт экземпляр класса <see cref="DialogServiceInteractionSource"/>.</summary>
	/// <param name="viewModel">Окно для построения взаимодействий.</param>
	public DialogServiceInteractionSource(TViewModel viewModel)
	{
		_dialogReactiveWindow = new(viewModel)
		{
			IconProvider = Locator.Current.GetService<IApplicationIconProvider>(),
		};
	}

	/// <summary>Регистрация обработчиков взаимодействий.</summary>
	/// <param name="dialogServiceInteractions">Сервис взаимодействий.</param>
	public void Register(DialogServiceInteractions dialogServiceInteractions)
	{
		_disposeInteractions =
		[
			dialogServiceInteractions.ShowQuestionMessage.RegisterHandler(_dialogReactiveWindow.ShowQuestionMessage),
			dialogServiceInteractions.ShowErrorMessage.RegisterHandler(_dialogReactiveWindow.ShowErrorMessage),
			dialogServiceInteractions.CloseWindow.RegisterHandler(_dialogReactiveWindow.CloseWindow),
		];
	}

	#region Disposable

	/// <summary></summary>
	public bool IsDisposed { get; set; }

	/// <summary></summary>
	public void Dispose()
	{
		if(!IsDisposed)
		{
			_disposeInteractions?.ForEach(x => x.Dispose());
			_disposeInteractions?.Clear();

			IsDisposed = true;
		}
	}

	#endregion
}
