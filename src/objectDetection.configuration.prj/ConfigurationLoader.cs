using System.Text.Json;

namespace ObjectDetection.Configuration;

/// <summary>Загрузка конфигурации приложения.</summary>
public class ProcessConfiguration
{
	private const string ApplicationDir = @"\objectDetection.application\Configuration";
	private const string FileName = @"\configuration.json";

	private readonly string _baseDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

	/// <summary>Выполняет чтение конфигурации из файла.</summary>
	public ApplicationConfiguration Load()
	{
		if(!Directory.Exists(_baseDir + ApplicationDir))
		{
			new DirectoryInfo(_baseDir + ApplicationDir).Create();
			throw new FileNotFoundException("Configuration not found");
		}

		if(!File.Exists(_baseDir + ApplicationDir + FileName))
		{
			new DirectoryInfo(_baseDir + ApplicationDir).Create();
			throw new FileNotFoundException("Configuration not found");
		}

		try
		{
			using var json = new FileStream(_baseDir + ApplicationDir + FileName, FileMode.OpenOrCreate);

			return JsonSerializer.Deserialize<ApplicationConfiguration>(json);
		}
		catch(Exception)
		{
			throw;
		}
	}
}
