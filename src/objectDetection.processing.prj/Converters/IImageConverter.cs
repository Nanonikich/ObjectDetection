namespace ObjectDetection.Processing.Converters;

/// <summary>Ковертер изображений.</summary>
public interface IImageConverter
{
	/// <summary>Конвертация изображения в байты.</summary>
	/// <param name="imageIn">Входящее изображение.</param>
	/// <returns>Обработанное изображение в байтах.</returns>
	byte[] SixLaborsImageToArray(SixLabors.ImageSharp.Image imageIn);
}
