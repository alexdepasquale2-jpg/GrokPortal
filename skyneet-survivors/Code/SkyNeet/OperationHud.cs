public sealed partial class OperationHud : PanelComponent
{
	OperationDirector D => Scene.GetAllComponents<OperationDirector>().FirstOrDefault();

	string ClockText
	{
		get
		{
			if ( D is null )
				return "--:--";
			var t = MathF.Max( D.TimeLeft, 0f );
			return $"{(int)(t / 60):00}:{(int)(t % 60):00}";
		}
	}

	string ScrapText => D is null ? "0" : D.Scrap.ToString();
	string NodeText => D is not null && D.NodeUp ? "ONLINE" : "DARK";
	string LoudText => D is null ? "0" : D.Loudness.ToString( "0.0" );
	string SancientText => D is not null && D.SancientActive ? "SANCIENT AWAKE" : "";
	string OwnerText => D?.SiteOwner ?? "?";
	string EndText => D is not null && D.OperationEnded ? $"ENDED {D.EndReason}" : "";

	protected override int BuildHash() =>
		HashCode.Combine( ClockText, ScrapText, NodeText, LoudText, SancientText, OwnerText, EndText );
}
