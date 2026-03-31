using System;
using System.Text.Json.Serialization;

namespace FlightTracker.Models;

public class Flight
{
	[JsonPropertyName("flightNumber")]
	public string FlightNumber { get; set; } = string.Empty;

	[JsonPropertyName("airlineName")]
	public string AirlineName { get; set; } = string.Empty;

	[JsonPropertyName("airlineCode")]
	public string AirlineCode { get; set; } = string.Empty;

	[JsonPropertyName("departureAirport")]
	public string DepartureAirport { get; set; } = string.Empty;

	[JsonPropertyName("arrivalAirport")]
	public string ArrivalAirport { get; set; } = string.Empty;

	[JsonPropertyName("scheduledDeparture")]
	public DateTime ScheduledDeparture { get; set; }

	[JsonPropertyName("scheduledArrival")]
	public DateTime ScheduledArrival { get; set; }

	[JsonPropertyName("aircraftType")]
	public string AircraftType { get; set; } = string.Empty;

	[JsonPropertyName("status")]
	public string Status { get; set; } = string.Empty;
}
