using ProtectEarth.Core.Interfaces;
using ProtectEarth.Core.Utils;
using ProtectEarth.Components;

using Godot;

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

		// Gameplay parameters for scoring and collision damage (assigned via editor).
		[ExportGroup("Gameplay")]
		[Export] public int ScoreBaseValue { get; private set; } = 300;
		[Export] public int DamageToPlayer { get; private set; } = 20;
		[Export] public int DamageToPlanet { get; private set; } = 10;

		// Internal state for movement behavior.
		private float _rotationSpeed;
		private Vector2 _center;

		// Convenience proxy — always reflects the current HealthComponent state.
		private bool IsDead => Health.IsDead;
		private void ForceKill() => Health.Kill();

		// IDamageable — delegated to HealthComponent.
		public void ApplyDamage(int damage) => Health.ApplyDamage(damage);

		// External hook — used by difficulty scaling systems.
		public void AddSpeed(float additionalSpeed) => Speed.AddSpeed(additionalSpeed);

		// ------------------------------------- Godot overrides ------------------------------------

		public override void _Ready()
		{
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

		public override void _ExitTree() => DisconnectSignals();

		// ------------------------------------ Signal management -----------------------------------

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

		// ------------------------------------ Signal handlers -------------------------------------

		private void OnBodyEntered(Node2D entity)
		{
			// Handles collisions with physics bodies (e.g., player).
			if (entity is IDamageable damageable)
				damageable.ApplyDamage(DamageToPlayer);

			ForceKill();
		}

		private void OnAreaEntered(Area2D entity)
		{
			// Ignore other asteroids to prevent chain-reaction collisions.
			if (entity.IsInGroup("Asteroid")) return;

			// Handles interactions with non-body entities (e.g., planet hitboxes).
			if (entity is IDamageable damageable)
				damageable.ApplyDamage(DamageToPlanet);

			ForceKill();
		}

		private void OnDeath()
		{
			// Disable collision immediately to prevent further interactions during death animation.
			Collision.SetDeferred("disabled", true);

			// Trigger explosion animation and notify scoring systems.
			AnimatedSprite.Play("explode");
			EmitSignal(SignalName.AsteroidDestroyed, ScoreBaseValue);
		}

		private void OnAnimationFinished()
		{
			// Cleanup only after explosion animation completes.
			if (AnimatedSprite.Animation != "explode") return;
			QueueFree();
		}

		// ------------------------------------ Movement logic --------------------------------------

		private void MoveTowardsCenter(double delta)
		{
			// Moves asteroid towards screen center with slight rotational drift.
			var direction = (_center - GlobalPosition).Normalized();
			GlobalPosition += direction * Speed.CurrentSpeed * (float)delta;
			Rotation += _rotationSpeed;
		}
	}
}