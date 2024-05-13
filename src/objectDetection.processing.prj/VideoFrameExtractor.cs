using MediaFileProcessor.Extensions;
using MediaFileProcessor.Models.Common;
using MediaFileProcessor.Processors;

namespace ObjectDetection.Processing;

/// <summary>Сегментатор видео на кадры.</summary>
public class VideoFrameExtractor
{
	private readonly List<string> _frameExtensions = [".png", ".jpg", ".bmp"];
	private readonly VideoFileProcessor _videoFileProcessor;

	/// <summary>Событие получения кадра из видео.</summary>
	public event EventHandler<byte[]> FrameCaptureEvent = delegate { };

	/// <summary>Создаёт экземпляр класса <see cref="VideoFrameExtractor"/>.</summary>
	public VideoFrameExtractor()
	{
		var directoryPath = Environment.CurrentDirectory;
		var ffmpegPath = directoryPath[..directoryPath.IndexOf("objectDetection")] + $"tools\\ffmpeg-7.0-essentials_build\\";

		_videoFileProcessor = new VideoFileProcessor($"{ffmpegPath}ffmpeg.exe", $"{ffmpegPath}ffprobe.exe");
	}

	/// <summary>Запускает процесс сегментации видео на кадры.</summary>
	/// <param name="videoPath">Путь к видео.</param>
	/// <param name="cancellationToken">Токен отмены.</param>
	public async Task FrameExtractorProcessing(string videoPath, CancellationToken cancellationToken)
	{
		if(_frameExtensions.Contains(Path.GetExtension(videoPath)))
		{
			FrameCaptureEvent?.Invoke(this, videoPath.ToBytes());
		}
		else
		{
			var mediaFile = new MediaFile(videoPath);
			var videoInfo = await _videoFileProcessor.GetVideoInfoAsync(mediaFile, cancellationToken);
			var formatInfo = videoInfo[videoInfo.IndexOf("\"format\": {", 0)..];

			string key = "\"duration\"";
			formatInfo = formatInfo.Remove(0, formatInfo.IndexOf(key) + key.Length).Replace(" ", "");
			formatInfo = formatInfo.Trim()[..(formatInfo.Trim().IndexOf(',') - 1)].Replace(":\"", "");
			var duration = TimeSpan.FromSeconds(double.Parse(formatInfo, System.Globalization.CultureInfo.InvariantCulture));

			for(TimeSpan i = default; i < duration; i = i.Add(new TimeSpan(0, 0, 5)))
			{
				if(!cancellationToken.IsCancellationRequested)
				{
					var frame = await _videoFileProcessor.ExtractFrameFromVideoAsync(
					i,
					mediaFile,
					null,
					MediaFileProcessor.Models.Enums.FileFormatType.JPG,
					cancellationToken);

					FrameCaptureEvent?.Invoke(this, frame.ToArray());

					frame.Dispose();
				}
				else
				{
					break;
				}
			}
		}
	}
}
