using Godot;
using ProtectEarth.Components;
using ProtectEarth.Utils;

namespace ProtectEarth.Entities
{
	public partial class Asteroid : RigidBody2D
	{
		[Export]
		public HealthComponent Health;

		[Export]
		public SpeedComponent Speed;

		private Vector2 _center;
		private float _rotationSpeed;

		public AnimatedSprite2D AnimatedSprite;


		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Health ??= GetNodeOrNull<HealthComponent>("HealthComponent");
			Speed ??= GetNodeOrNull<SpeedComponent>("SpeedComponent");
			AnimatedSprite ??= GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite");

			_center = ScreenUtils.GetScreenCenter(this);
			_rotationSpeed = (float)GD.RandRange(-0.01f, 0.01f);
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
			Vector2 direction = (_center - GlobalPosition).Normalized();

			LinearVelocity = direction * Speed.CurrentSpeed;
			Rotation += _rotationSpeed;
		}

		public void Die()
		{
			AnimatedSprite?.Play("explode");
		}

		public void OnAnimationFinished()
		{
			QueueFree();
		}
	}
}
