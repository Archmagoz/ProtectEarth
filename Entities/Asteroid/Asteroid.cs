using Godot;
using ProtectEarth.Utils;
using ProtectEarth.Components;

namespace ProtectEarth.Entities
{
	public partial class Asteroid : RigidBody2D
	{
		// Node references (assigned via editor or auto-resolved in _Ready)
		[Export] public AnimatedSprite2D AnimatedSprite;
		[Export] public CollisionPolygon2D Collision;
		[Export] public HealthComponent Health;
		[Export] public SpeedComponent Speed;

		private bool _isDead = false;
		private float _rotationSpeed;
		private Vector2 _direction;
		private Vector2 _center;

		public bool IsDead => _isDead;

		public override void _Ready()
		{
			// Fallback to find nodes if not set via editor.
			AnimatedSprite ??= GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite");
			Collision ??= GetNodeOrNull<CollisionPolygon2D>("Collision");
			Health ??= GetNodeOrNull<HealthComponent>("HealthComponent");
			Speed ??= GetNodeOrNull<SpeedComponent>("SpeedComponent");

			// Cache screen center and randomize rotation speed for variation.
			_center = ScreenUtils.GetScreenCenter(this);
			_rotationSpeed = RNG.Range(-0.01f, 0.01f);

			// Connect signals.
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

		// Called via AnimatedSprite animation_finished signal to remove the asteroid.
		public void OnAnimationFinished()
		{
			QueueFree();
		}

		// ----------------------------- Main loop -----------------------------

		// Moves the asteroid toward the screen center while applying rotation.
		private void MoveTowardsCenter()
		{
			_direction = (_center - GlobalPosition).Normalized();
			LinearVelocity = _direction * Speed.CurrentSpeed;
			Rotation += _rotationSpeed;
		}

		public override void _PhysicsProcess(double delta)
		{
			if (_isDead) return;
			MoveTowardsCenter();
		}
	}
}
