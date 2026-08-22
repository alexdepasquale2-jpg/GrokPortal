public sealed class CustomTopDownController : Component
{
	[RequireComponent] PlayerController Controller { get; set; }

	protected override void OnFixedUpdate()
	{
		// The operation is over: extracted, dead or out of clock. Nothing you do now counts,
		// so stop pretending it might. The camera keeps looking at what you caused.
		var director = Scene.GetAllComponents<OperationDirector>().FirstOrDefault();
		if ( director is not null && director.OperationEnded )
		{
			Controller.WishVelocity = Vector3.Zero;
			return;
		}

		// Lets make the player move when we press WASD
		var speed = Input.Down( "Run" ) ? Controller.RunSpeed : Controller.WalkSpeed;
		Controller.WishVelocity = Input.AnalogMove * speed;

		// And rotate the player to face the direction of the movement
		if ( Controller.WishVelocity.LengthSquared > 0 )
		{
			var targetAngle = Controller.WishVelocity.EulerAngles;
			Controller.EyeAngles = Rotation.Slerp( Controller.EyeAngles, targetAngle, Time.Delta * 10f );
		}
	}

	protected override void OnPreRender()
	{
		// This will update the camera's position so that it's up in the air and back a bit, looking down at an angle.
		Scene.Camera.WorldPosition = Controller.WorldPosition + Vector3.Up * 1024f + Vector3.Backward * 256f;
		Scene.Camera.WorldRotation = new Angles( 75, 0, 0 );
	}
}



