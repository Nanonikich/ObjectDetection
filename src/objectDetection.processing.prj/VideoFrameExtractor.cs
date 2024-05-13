using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV;

namespace ObjectDetection.Processing;

/// <summary>Сегментатор видео на кадры.</summary>
public class VideoFrameExtractor : IDisposable
{
	private VideoCapture _capture;

	/// <summary>Событие получения кадра из видео.</summary>
	public event EventHandler<byte[]> FrameCaptureEvent = delegate { };

	#region Methods

	/// <summary>Загрузчик процесса сегментации видео на кадры.</summary>
	/// <param name="videoPath">Путь к видео.</param>
	/// <param name="cancellationToken">Токен отмены.</param>
	/// <returns>Список кадров в виде byte[].</returns>
	public void InitializeFrameExtractor(string videoPath, CancellationToken cancellationToken)
	{
		_capture = new(videoPath);

		var framesCount = (int)_capture.Get(CapProp.FrameCount);

		for(int i = 1; i <= framesCount; i++)
		{
			if(!cancellationToken.IsCancellationRequested)
			{
				ReceivingFrames();
			}
			else
			{
				break;
			}
		}
	}

	private bool ReceivingFrames()
	{
		using var frame = _capture.QueryFrame();

		if(frame != null)
		{
			var byteFrame = frame.ToImage<Bgr, byte>().ToJpegData();
			FrameCaptureEvent?.Invoke(this, byteFrame);

			frame?.Dispose();

			return true;
		}
		else
		{
			Dispose();
		}

		frame?.Dispose();

		return false;
	}

	#endregion

	/// <inheritdoc/>
	public void Dispose() => _capture?.Dispose();

}
