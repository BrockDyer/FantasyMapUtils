using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using VoronoiModel;
using VoronoiModel.Services;
using static System.Double;

namespace MarketAreas.Views.Popups;

public partial class PointInputPopup
{

	private readonly Action<VoronoiPoint> addPointAction;

	public PointInputPopup(Action<VoronoiPoint> addPointAction)
	{
		InitializeComponent();
		this.addPointAction = addPointAction;
	}

	private void ClearEntries()
	{
		PointNameEntry.ClearValue(Entry.TextProperty);
		PointWeightEntry.ClearValue(Entry.TextProperty);
	}

	private void OnAddPointClicked(object sender, EventArgs e)
	{
		var name = PointNameEntry.Text;
        var point = new VoronoiPoint
		{
			Name = name
		};

        if (TryParse(PointWeightEntry.Text, out var weight))
        {
			point.Weight = weight;
        }

        Debug.WriteLine($"Adding point {point}");
		addPointAction(point);
		ClearEntries();
		Reset();
	}

	private void OnPointNameInput(object sender, EventArgs e)
	{
		var nameEntry = (Entry)sender;
		Debug.WriteLine(nameEntry.Text);
	}

	private void OnPointWeightInput(object sender, EventArgs e)
	{
		var weightEntry = (Entry)sender;
		Debug.WriteLine(weightEntry.Text);
	}

	private void OnCancelClicked(object sender, EventArgs e) => Close();
}
