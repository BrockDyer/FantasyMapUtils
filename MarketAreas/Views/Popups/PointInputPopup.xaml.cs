using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Maui.Views;

using VoronoiModel;
using VoronoiModel.Services;

namespace MarketAreas.Views.Popups;

public partial class PointInputPopup : Popup
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

        Decimal weight;
        if (Decimal.TryParse(PointWeightEntry.Text, out weight))
        {
			point.Weight = weight;
        }

        Debug.WriteLine(string.Format("Adding point {0}", point));
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
