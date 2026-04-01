using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Mapsui;
using Mapsui.Projections;
using Mapsui.Tiling;
using Mapsui.UI.Avalonia;

namespace FlightTracker.Views;

public partial class View1 : UserControl
{
	private bool _mapInitialized;

	public View1()
	{
		InitializeComponent();
		Loaded += OnLoaded;
	}

	private void OnLoaded(object? sender, RoutedEventArgs e)
	{
		if (_mapInitialized)
		{
			return;
		}

		var mapControl = this.FindControl<MapControl>("RouteMapControl");
		if (mapControl is null)
		{
			return;
		}

		mapControl.Map ??= new Map();

		mapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
		var center = SphericalMercator.FromLonLat(10.0, 55.0);
		mapControl.Map.Navigator.CenterOn(center.x, center.y, 0, null!);
		mapControl.Map.Navigator.ZoomToLevel(4);
		_mapInitialized = true;
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}
