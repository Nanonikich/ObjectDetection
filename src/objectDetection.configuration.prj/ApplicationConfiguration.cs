using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ObjectDetection.Configuration;

/// <summary>Конфигурация приложения.</summary>
[DataContract]
public record ApplicationConfiguration
{
	/// <summary>Нейросети.</summary>
	[DataMember]
	[JsonPropertyName("Networks")]
	public Networks Networks { get; set; }
}

/// <summary>Нейросети.</summary>
[DataContract]
public record Networks
{
	/// <summary>Детектор.</summary>
	[DataMember]
	[JsonPropertyName("detector")]
	public Detector Detector { get; set; }
}

/// <summary>Детектор.</summary>
[DataContract]
public record Detector
{
	/// <summary>Местоположение детектора.</summary>
	[DataMember]
	[JsonPropertyName("detector_path")]
	public string? DetectorPath { get; set; }
}