using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp;

namespace ObjectDetection.Processing.Converters;

/// <summary>Конвертер изображений.</summary>
public class ImageConverter : IImageConverter
{
	/// <inheritdoc/>
	public byte[] SixLaborsImageToArray(SixLabors.ImageSharp.Image imageIn)
	{
		using var ms = new MemoryStream();
		imageIn.Save(ms, JpegFormat.Instance);

		return ms.ToArray();
	}
}
