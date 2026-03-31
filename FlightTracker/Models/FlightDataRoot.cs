using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FlightTracker.Models;

public class FlightDataRoot
{
	[JsonPropertyName("airports")]
	public List<Airport> Airports { get; set; } = [];

	[JsonPropertyName("flights")]
	public List<Flight> Flights { get; set; } = [];
}
