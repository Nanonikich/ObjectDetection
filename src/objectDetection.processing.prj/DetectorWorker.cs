using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;

using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;

using ObjectDetection.Processing.Helpers;
using ObjectDetection.Gui.Helpers;

namespace ObjectDetection.Processing;

/// <summary>Детектор объектов.</summary>
public class DetectorWorker
{
	const float MinConfidence = 0.7f;

	/// <summary>Обработка кадра.</summary>
	/// <param name="onnxPath">Путь к модели onnx.</param>
	/// <param name="original">Исходное изображение.</param>
	/// <returns>Результаты обработки.</returns>
	public VideoProcessingResults FrameProcessing(string onnxPath, byte[]? original)
	{
		var image = Image.Load<Rgb24>(original);

		// изменение размеров изображения
		float ratio = 800f / Math.Min(image.Width, image.Height);
		image.Mutate(x => x.Resize((int)(ratio * image.Width), (int)(ratio * image.Height)));

		// предварительная обработка изображения
		var paddedHeight = (int)(Math.Ceiling(image.Height / 32f) * 32f);
		var paddedWidth = (int)(Math.Ceiling(image.Width / 32f) * 32f);
		var input = new DenseTensor<float>([3, paddedHeight, paddedWidth]);
		var mean = new[] { 102.9801f, 115.9465f, 122.7717f };
		image.ProcessPixelRows(accessor =>
		{
			for(int y = paddedHeight - accessor.Height; y < accessor.Height; y++)
			{
				Span<Rgb24> pixelSpan = accessor.GetRowSpan(y);
				for(int x = paddedWidth - accessor.Width; x < accessor.Width; x++)
				{
					input[0, y, x] = pixelSpan[x].B - mean[0];
					input[1, y, x] = pixelSpan[x].G - mean[1];
					input[2, y, x] = pixelSpan[x].R - mean[2];
				}
			}
		});

		using var inputOrtValue = OrtValue.CreateTensorValueFromMemory(OrtMemoryInfo.DefaultInstance, 
			input.Buffer, [3, paddedHeight, paddedWidth]);

		// входные данные для модели
		var inputs = new Dictionary<string, OrtValue>
		{
			{ "image", inputOrtValue }
		};

		// вывод
		using var session = new InferenceSession(onnxPath);
		using var runOptions = new RunOptions();
		using IDisposableReadOnlyCollection<OrtValue> results = session.Run(runOptions, inputs, session.OutputNames);

		// постобработка и получение предсказаний
		var resultsArray = results.ToArray();
		var boxes = results[0].GetTensorDataAsSpan<float>();
		var labels = results[1].GetTensorDataAsSpan<long>();
		var confidences = results[2].GetTensorDataAsSpan<float>();

		var predictions = new List<Prediction>();

		for(int i = 0; i < boxes.Length - 4; i += 4)
		{
			var index = i / 4;
			if(confidences[index] >= MinConfidence)
			{
				predictions.Add(new Prediction
				{
					Box = new Box(boxes[i], boxes[i + 1], boxes[i + 2], boxes[i + 3]),
					Label = LabelMap.Labels[labels[index]],
					Confidence = confidences[index]
				});
			}
		}

		var newDetectionResults = new List<ObjectInfo>();

		// размещение полей, меток и элементов уверенности на изображение и сохранение для просмотра
		Font font = SystemFonts.CreateFont("Arial", 16);
		foreach(var p in predictions)
		{
			image.Mutate(x =>
			{
				x.DrawLine(Color.Red, 2f,
				[
					new(p.Box.Xmin, p.Box.Ymin),
					new(p.Box.Xmax, p.Box.Ymin),

					new(p.Box.Xmax, p.Box.Ymin),
					new(p.Box.Xmax, p.Box.Ymax),

					new(p.Box.Xmax, p.Box.Ymax),
					new(p.Box.Xmin, p.Box.Ymax),

					new(p.Box.Xmin, p.Box.Ymax),
					new(p.Box.Xmin, p.Box.Ymin)
				]);
				x.DrawText($"{p.Label}, {p.Confidence:0.00}", font, Color.White, new PointF(p.Box.Xmin, p.Box.Ymin));
			});

			newDetectionResults.Add(new()
			{
				Title = p.Label,
				Result = $"{p.Confidence:0.00}"
			});
		}

		return new VideoProcessingResults(image, newDetectionResults);
	}
}