using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

using ObjectDetection.Processing.Helpers;

namespace ObjectDetection.Gui.Helpers;

/// <summary>Данные для обработки.</summary>
public record VideoProcessingResults : IDisposable
{
	/// <summary>Показываемое изображение.</summary>
	public Image<Rgb24>? Picture { get; private set; }

	/// <summary>Результаты детекций.</summary>
	public List<ObjectInfo> DetectionResults { get; }

    public VideoProcessingResults(Image<Rgb24>? imageRes, List<ObjectInfo> detectionResults)
    {
		Picture = imageRes;
		DetectionResults = detectionResults;
	}

    public void Dispose()
	{
		Picture?.Dispose();
		Picture = null;
	}
}
