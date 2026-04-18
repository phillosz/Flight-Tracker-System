using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FlightTracker.ViewModels;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Tiling;
using Mapsui.UI.Avalonia;
using Mapsui.Nts;
using Mapsui.Styles;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace FlightTracker.Views;

public partial class View1 : UserControl
{
	private bool _mapInitialized;
	private MemoryLayer? _routesLayer;

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
		_routesLayer ??= new MemoryLayer { Name = "Routes" };
		mapControl.Map.Layers.Add(_routesLayer);

		if (DataContext is View1ViewModel vm)
		{
			vm.FlightPathsUpdated += UpdateRoutesLayer;
		}

		var center = SphericalMercator.FromLonLat(10.0, 55.0);
		mapControl.Map.Navigator.CenterOn(center.x, center.y, 0, null!);
		mapControl.Map.Navigator.ZoomToLevel(4);
		_mapInitialized = true;
	}

	private void UpdateRoutesLayer()
	{
		if (_routesLayer is null)
		{
			return;
		}

		if (DataContext is not View1ViewModel vm)
		{
			return;
		}

		var features = new List<IFeature>();
		foreach (var path in vm.FlightPaths)
		{
			var origin = SphericalMercator.FromLonLat(path.oLon, path.oLat);
			var destination = SphericalMercator.FromLonLat(path.dLon, path.dLat);

			var line = new LineString(new[]
			{
				new Coordinate(origin.x, origin.y),
				new Coordinate(destination.x, destination.y)
			});

			features.Add(new GeometryFeature(line));
		}

		_routesLayer.Features = features;
		_routesLayer.Style = new VectorStyle
		{
			Line = new Pen(Color.Red, 2)
		};
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}
