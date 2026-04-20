using Godot;
using ProtectEarth.Components;
using ProtectEarth.Utils;

namespace ProtectEarth.Entities
{
	public partial class Asteroid : RigidBody2D
	{
		[Export] public AnimatedSprite2D AnimatedSprite;
		[Export] public HealthComponent Health;
		[Export] public SpeedComponent Speed;

		private float _rotationSpeed;
		private Vector2 _center;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Health ??= GetNodeOrNull<HealthComponent>("HealthComponent");
			Speed ??= GetNodeOrNull<SpeedComponent>("SpeedComponent");
			AnimatedSprite ??= GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite");

			_center = ScreenUtils.GetScreenCenter(this);
			_rotationSpeed = RNG.Range(-0.01f, 0.01f);
		}


		// Moves the asteroid towards the center of the screen.
		private void MoveTowardsCenter()
		{
			Vector2 direction = (_center - GlobalPosition).Normalized();

			LinearVelocity = direction * Speed.CurrentSpeed;
			Rotation += _rotationSpeed;
		}

		// Plays the explosion animation.
		public void Die()
		{
			AnimatedSprite?.Play("explode");
		}

		// Called when the explosion animation finishes.
		public void OnAnimationFinished()
		{
			QueueFree();
		}

		// Called during the physics processing.
		public override void _PhysicsProcess(double delta)
		{
			MoveTowardsCenter();
		}
	}
}
