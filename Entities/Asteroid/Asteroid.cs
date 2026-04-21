using Godot;
using ProtectEarth.Utils;
using ProtectEarth.Components;

namespace ProtectEarth.Entities
{
	public partial class Asteroid : RigidBody2D
	{
		// Node references (assigned via editor or auto-resolved in _Ready).
		[Export] public AnimatedSprite2D AnimatedSprite { get; private set; }
		[Export] public CollisionPolygon2D Collision { get; private set; }
		[Export] public HealthComponent Health { get; private set; }
		[Export] public SpeedComponent Speed { get; private set; }

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
			AnimatedSprite.AnimationFinished += OnAnimationFinished;
			Health.Death += OnDeath;
		}

		public override void _PhysicsProcess(double delta)
		{
			if (_isDead) return;
			MoveTowardsCenter();
		}

		// ------------------------------ Signal handlers ----------------------------------

		// Handles death: stop movement and play explosion animation.
		private void OnDeath()
		{
			_isDead = true;
			Collision.Disabled = true;
			LinearVelocity = Vector2.Zero;
			AnimatedSprite.Play("explode");
		}

		// Once the explosion animation finishes, remove the asteroid from the scene.
		private void OnAnimationFinished()
		{
			if (AnimatedSprite.Animation != "explode") return;
			QueueFree();
		}

		// ------------------------------ Movement logic ----------------------------------

		// Moves the asteroid toward the screen center while applying rotation.
		private void MoveTowardsCenter()
		{
			_direction = (_center - GlobalPosition).Normalized();
			LinearVelocity = _direction * Speed.CurrentSpeed;
			Rotation += _rotationSpeed;
		}
	}
}
