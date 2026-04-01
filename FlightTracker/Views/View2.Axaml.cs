using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FlightTracker.Views;

public partial class View2 : UserControl
{
	public View2()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}
