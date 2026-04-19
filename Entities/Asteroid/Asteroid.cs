using Godot;

namespace ProtectEarth.Entities
{
	using ProtectEarth.Components;
	using ProtectEarth.Utils;

	public partial class Asteroid : RigidBody2D
	{
		[Export]
		public HealthComponent Health;

		[Export]
		public SpeedComponent Speed;


		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Health ??= GetNodeOrNull<HealthComponent>("HealthComponent");
			Speed ??= GetNodeOrNull<SpeedComponent>("SpeedComponent");
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		// Called during the physics processing.
		public override void _PhysicsProcess(double delta)
		{
			MoveTowardsCenter();
		}

		// Moves the asteroid towards the center of the screen.
		private void MoveTowardsCenter()
		{
			Vector2 center = ScreenUtils.GetScreenCenter(this);
			Vector2 direction = (center - GlobalPosition).Normalized();

			LinearVelocity = direction * Speed.CurrentSpeed;
		}
	}
}
