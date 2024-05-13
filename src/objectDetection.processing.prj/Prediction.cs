namespace ObjectDetection.Processing;

/// <summary>Предсказание детекции.</summary>
public class Prediction
{
	/// <summary>Ограничивающий BBox объекта.</summary>
	public Box Box { get; set; }

	/// <summary>Метка объекта.</summary>
	public string Label { get; set; }

	/// <summary>Точность предсказания.</summary>
	public float Confidence { get; set; }
}

/// <summary>Ограничивающий BBox объекта.</summary>
/// <remarks>Создаёт экземпляр класса <see cref="Box"/>.</remarks>
/// <param name="xmin">Минимальная точка X.</param>
/// <param name="ymin">Минимальная точка Y.</param>
/// <param name="xmax">Максимальная точка X.</param>
/// <param name="ymax">Максимальная точка Y.</param>
public class Box(float xmin, float ymin, float xmax, float ymax)
{
	/// <summary>Минимальная точка X.</summary>
	public float Xmin { get; set; } = xmin;

	/// <summary>Минимальная точка Y.</summary>
	public float Ymin { get; set; } = ymin;

	/// <summary>Максимальная точка X.</summary>
	public float Xmax { get; set; } = xmax;

	/// <summary>Максимальная точка Y.</summary>
	public float Ymax { get; set; } = ymax;
}
