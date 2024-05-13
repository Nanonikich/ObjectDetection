namespace ObjectDetection.Gui.Common.Helpers;

/// <summary>Текст сообщения.</summary>
public record MessageContent
{
	/// <summary>Заголовок сообщения.</summary>
	public string Title { get; set; }

	/// <summary>Информация.</summary>
	public string Message { get; set; }
}