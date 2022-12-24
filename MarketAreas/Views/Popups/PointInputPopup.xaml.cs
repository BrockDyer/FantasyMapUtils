using System.Diagnostics;
using CommunityToolkit.Maui.Views;

using VoronoiModel;

namespace MarketAreas.Views.Popups;

public partial class PointInputPopup : Popup
{
	public PointInputPopup()
	{
		InitializeComponent();
	}

	private void OnAddPointClicked(object sender, EventArgs e)
	{
		var name = PointNameEntry.Text;
		var weight = Decimal.Parse(PointWeightEntry.Text);
		var point = new VoronoiPoint
		{
			Name = name,
			Weight = weight
		};
		Debug.WriteLine(string.Format("Adding point {0}", point));
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
}
