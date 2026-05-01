using ProtectEarth.Core.Interfaces;
using ProtectEarth.Core.Utils;
using ProtectEarth.Components;

using Godot;
using System;

namespace ProtectEarth.Entities
{
	public partial class Asteroid : Area2D, IDamageable
	{
		// Signal Handler.
		[Signal] public delegate void AsteroidDestroyedEventHandler(int scoreValue);

		// Node references (assigned via editor).
		[ExportGroup("Components")]
		[Export] public AnimatedSprite2D AnimatedSprite { get; private set; }
		[Export] public CollisionPolygon2D Collision { get; private set; }
		[Export] public HealthComponent Health { get; private set; }
		[Export] public SpeedComponent Speed { get; private set; }

		// Gameplay parameters for scoring and player damage (assigned via editor).
		[ExportGroup("Gameplay")]
		[Export] public int ScoreBaseValue { get; private set; } = 300;
		[Export] public int DamageToPlayer { get; private set; } = 20;
		[Export] public int DamageToPlanet { get; private set; } = 10;

		// Internal state for rotation and screen center caching.
		private float _rotationSpeed;
		private Vector2 _center;

		// Convenience proxy — always reflects the current HealthComponent state.
		private bool IsDead => Health.IsDead;
		private void ForceKill() => Health.Kill();

		// IDamageable delegate to HealthComponent.
		public void ApplyDamage(int damage) => Health.ApplyDamage(damage);

		// Delegate AddSpeed to SpeedComponent, used by GameManager on difficulty changes.
		public void AddSpeed(float additionalSpeed) => Speed.AddSpeed(additionalSpeed);

		// ------------------------------ Godot overrides ----------------------------------

		public override void _Ready()
		{
			// Hard validation — these components are required for the Asteroid to function.
			// Throws in all build configurations, ensuring misconfigured scenes are caught early.
			if (AnimatedSprite == null)
				throw new InvalidOperationException("AnimatedSprite is not assigned on Asteroid.");
			if (Collision == null)
				throw new InvalidOperationException("Collision is not assigned on Asteroid.");
			if (Health == null)
				throw new InvalidOperationException("HealthComponent is not assigned on Asteroid.");
			if (Speed == null)
				throw new InvalidOperationException("SpeedComponent is not assigned on Asteroid.");

			_center = ScreenUtils.GetScreenCenter(this);
			_rotationSpeed = RNG.Range(-0.01f, 0.01f);

			AddToGroup("Asteroid");
			ConnectSignals();
		}

		public override void _PhysicsProcess(double delta)
		{
			if (IsDead) return;
			MoveTowardsCenter(delta);
		}

		public override void _ExitTree()
		{
			DisconnectSignals();
		}

		// ------------------------------ Signal management ----------------------------------

		private void ConnectSignals()
		{
			AnimatedSprite.AnimationFinished += OnAnimationFinished;
			BodyEntered += OnBodyEntered;
			AreaEntered += OnAreaEntered;
			Health.Death += OnDeath;
		}

		private void DisconnectSignals()
		{
			AnimatedSprite.AnimationFinished -= OnAnimationFinished;
			BodyEntered -= OnBodyEntered;
			AreaEntered -= OnAreaEntered;
			Health.Death -= OnDeath;
		}

		// ------------------------------ Signal handlers ----------------------------------

		private void OnBodyEntered(Node2D entity)
		{
			if (entity is IDamageable damageable)
				damageable.ApplyDamage(DamageToPlayer);

			ForceKill();
		}

		private void OnAreaEntered(Area2D entity)
		{
			if (entity.IsInGroup("Asteroid")) return;

			if (entity is IDamageable damageable)
				damageable.ApplyDamage(DamageToPlanet);

			ForceKill();
		}

		private void OnDeath()
		{
			Collision.SetDeferred("disabled", true);
			AnimatedSprite.Play("explode");
			EmitSignal(SignalName.AsteroidDestroyed, ScoreBaseValue);
		}

		private void OnAnimationFinished()
		{
			if (AnimatedSprite.Animation != "explode") return;
			QueueFree();
		}

		// ------------------------------ Movement logic ----------------------------------

		private void MoveTowardsCenter(double delta)
		{
			var direction = (_center - GlobalPosition).Normalized();
			GlobalPosition += direction * Speed.CurrentSpeed * (float)delta;
			Rotation += _rotationSpeed;
		}
	}
}