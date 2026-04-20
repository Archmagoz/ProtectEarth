using Godot;
using ProtectEarth.Utils;
using ProtectEarth.Components;

namespace ProtectEarth.Entities
{
	public partial class Asteroid : RigidBody2D
	{
		// References (set via editor or fallback in _Ready)
		[Export] public AnimatedSprite2D AnimatedSprite;
		[Export] public CollisionPolygon2D Collision;
		[Export] public HealthComponent Health;
		[Export] public SpeedComponent Speed;

		private float _rotationSpeed;
		private Vector2 _center;
		private bool _isDead = false;

		public bool IsDead => _isDead;

		public override void _Ready()
		{
			AnimatedSprite ??= GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite");
			Collision ??= GetNodeOrNull<CollisionPolygon2D>("Collision");
			Health ??= GetNodeOrNull<HealthComponent>("HealthComponent");
			Speed ??= GetNodeOrNull<SpeedComponent>("SpeedComponent");

			_center = ScreenUtils.GetScreenCenter(this);
			_rotationSpeed = RNG.Range(-0.01f, 0.01f);

			Health.Death += OnDeath;
		}

		// Handles death: stop movement and play explosion animation.
		private void OnDeath()
		{
			_isDead = true;
			Collision.Disabled = true;
			LinearVelocity = Vector2.Zero;
			AnimatedSprite?.Play("explode");
		}

		// Called when the explosion animation finishes to free the asteroid.
		public void OnAnimationFinished()
		{
			QueueFree();
		}

		private void MoveTowardsCenter()
		{
			Vector2 direction = (_center - GlobalPosition).Normalized();

			LinearVelocity = direction * Speed.CurrentSpeed;
			Rotation += _rotationSpeed;
		}

		public override void _PhysicsProcess(double delta)
		{
			if (_isDead) return;
			MoveTowardsCenter();
		}
	}
}
