using System.Text.Json.Serialization;

namespace FlightTracker.Models;

public class Airport
{
	[JsonPropertyName("iataCode")]
	public string IataCode { get; set; } = string.Empty;

	[JsonPropertyName("name")]
	public string Name { get; set; } = string.Empty;

	[JsonPropertyName("city")]
	public string City { get; set; } = string.Empty;

	[JsonPropertyName("country")]
	public string Country { get; set; } = string.Empty;

	[JsonPropertyName("latitude")]
	public double Latitude { get; set; }

	[JsonPropertyName("longitude")]
	public double Longitude { get; set; }
}
