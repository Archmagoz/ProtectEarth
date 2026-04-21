using ProtectEarth.Core.Interfaces;
using ProtectEarth.Core.Utils;
using ProtectEarth.Components;

using Godot;

namespace ProtectEarth.Entities
{
	public partial class Asteroid : RigidBody2D, IDamageable
	{
		// Node references (assigned via editor or auto-resolved in _Ready).
		[Export] public AnimatedSprite2D AnimatedSprite { get; private set; }
		[Export] public CollisionPolygon2D Collision { get; private set; }
		[Export] public HealthComponent Health { get; private set; }
		[Export] public SpeedComponent Speed { get; private set; }

		// Internal state for rotation and screen center caching.
		private float _rotationSpeed;
		private Vector2 _center;

		// IDamageable delegate to HealthComponent.
		public void ApplyDamage(int damage) => Health.ApplyDamage(damage);

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

		// Object main loop: handle movement and rotation.
		public override void _PhysicsProcess(double delta)
		{
			if (Health.IsDead) return; // early exit if dead
			MoveTowardsCenter();
		}

		// Clean up signal connections after the node ExitTree to prevent potential issues.
		public override void _ExitTree()
		{
			if (Health != null)
				Health.Death -= OnDeath;

			if (AnimatedSprite != null)
				AnimatedSprite.AnimationFinished -= OnAnimationFinished;
		}

		// ------------------------------ Signal handlers ----------------------------------

		// Handles death: stop movement and play explosion animation.
		private void OnDeath()
		{
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
			Vector2 direction = (_center - GlobalPosition).Normalized();
			LinearVelocity = direction * Speed.CurrentSpeed;
			Rotation += _rotationSpeed;
		}
	}
}
