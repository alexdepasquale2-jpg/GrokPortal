using Sandbox.UI;

/// <summary>
/// Screen HUD without Razor. Razor codegen was a separate class, so markup could not see C# members and the engine failed to boot.
/// </summary>
public sealed class OperationHud : PanelComponent
{
	Label _body;

	protected override void OnUpdate()
	{
		if ( Panel is null )
			return;

		if ( _body is null )
		{
			Panel.Style.Position = PositionMode.Absolute;
			Panel.Style.Left = 24;
			Panel.Style.Top = 24;
			Panel.Style.Padding = 12;
			Panel.Style.BackgroundColor = Color.Black.WithAlpha( 0.7f );
			Panel.Style.FontColor = Color.White;
			Panel.Style.FontSize = 18;
			Panel.Style.FontFamily = "Poppins";
			Panel.Style.WhiteSpace = WhiteSpace.PreWrap;
			_body = Panel.AddChild<Label>();
		}

		var d = Scene.GetAllComponents<OperationDirector>().FirstOrDefault();
		if ( d is null )
		{
			_body.Text = "SKYNEET";
			return;
		}

		var t = MathF.Max( d.TimeLeft, 0f );
		var clock = $"{(int)(t / 60):00}:{(int)(t % 60):00}";
		var node = d.NodeUp ? "ONLINE" : "DARK";
		var sancient = d.SancientActive ? "SANCIENT AWAKE" : "";
		var ended = d.OperationEnded ? $"ENDED {d.EndReason}" : "";

		_body.Text =
			$"SKYNEET  {clock}  |  SCRAP {d.Scrap}\n" +
			$"NNN {node}  LOUD {d.Loudness:0.0}  {sancient}\n" +
			$"OWNER {d.SiteOwner}  {ended}\n" +
			"E: plant NNN / raise fort   extract: hold the dark pad";
	}
}
