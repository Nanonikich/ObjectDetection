using System.Windows.Input;

using Avalonia;
using Avalonia.Controls;

namespace ObjectDetection.Gui.Common.Views.Controls.Components;

public partial class BaseButtonsView : UserControl
{
	public static readonly StyledProperty<ICommand> ApplyCommandProperty =
	   AvaloniaProperty.Register<BaseButtonsView, ICommand>(nameof(ApplyCommand));

	public static readonly StyledProperty<ICommand> CancelCommandProperty =
		AvaloniaProperty.Register<BaseButtonsView, ICommand>(nameof(CancelCommand));

	public ICommand ApplyCommand
	{
		get => GetValue(ApplyCommandProperty);
		set => SetValue(ApplyCommandProperty, value);
	}

	public ICommand CancelCommand
	{
		get => GetValue(CancelCommandProperty);
		set => SetValue(CancelCommandProperty, value);
	}

	public BaseButtonsView()
	{
		InitializeComponent();
	}
}
