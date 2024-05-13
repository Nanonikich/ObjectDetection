namespace ObjectDetection.Processing.Helpers;

/// <summary>Информация об обнаруженном объекте.</summary>
public record ObjectInfo
{
	/// <summary>Обнаруженный объект.</summary>
	public string Title { get; set; } = string.Empty;

	/// <summary>Точность обнаружения.</summary>
	public string Result { get; set; } = string.Empty;
}
