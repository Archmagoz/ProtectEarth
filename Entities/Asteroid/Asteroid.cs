using ProtectEarth.Core.Interfaces;
using ProtectEarth.Core.Utils;
using ProtectEarth.Components;

using Godot;

namespace ProtectEarth.Entities
{
	public partial class Asteroid : RigidBody2D, IDamageable
	{
		// Signal Handler.
		[Signal] public delegate void AsteroidDestroyedEventHandler(int scoreValue);

		// Node references (assigned via editor or auto-resolved in _Ready).
		[Export] public AnimatedSprite2D AnimatedSprite { get; private set; }
		[Export] public CollisionPolygon2D Collision { get; private set; }
		[Export] public HealthComponent Health { get; private set; }
		[Export] public SpeedComponent Speed { get; private set; }

		// Gameplay parameters for scoring and player damage, can be set via editor or code.
		[Export] public int ScoreBaseValue { get; private set; } = 300;
		[Export] public int DamageToPlayer { get; private set; } = 20;
		[Export] public int DificultyLevel { get; private set; } = 1;

		// Internal state for rotation and screen center caching.
		private float _rotationSpeed;
		private Vector2 _center;

		// IDamageable delegate to HealthComponent.
		public void ApplyDamage(int damage) => Health.ApplyDamage(damage);

		// Delegate AddSpeed to SpeedComponent, used by GameManager to increase asteroid speed on difficulty changes.
		public void AddSpeed(float additionalSpeed) => Speed.AddSpeed(additionalSpeed);

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

			AddToGroup("asteroids");
		}

		// Object main loop: handle movement and rotation.
		public override void _PhysicsProcess(double delta)
		{
			if (Health.IsDead) return; // early exit if dead
			MoveTowardsCenter();
		}

		// ------------------------------ Signal handlers ----------------------------------

		// Handles death: stop movement and play explosion animation.
		private void OnDeath()
		{
			LinearVelocity = Vector2.Zero;
			Collision.SetDeferred("disabled", true);
			AnimatedSprite.Play("explode");

			// Emit score value to be added to the player's score, factoring in difficulty level.
			EmitSignal(SignalName.AsteroidDestroyed, ScoreBaseValue * DificultyLevel);
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
			var direction = (_center - GlobalPosition).Normalized();
			LinearVelocity = direction * Speed.CurrentSpeed;
			Rotation += _rotationSpeed;
		}
	}
}
