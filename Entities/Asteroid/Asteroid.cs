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
		[Export] private AnimatedSprite2D _animatedSprite;
		[Export] private CollisionPolygon2D _collision;
		[Export] private HealthComponent _health;
		[Export] private SpeedComponent _speed;

		// Gameplay parameters for scoring and collision damage (assigned via editor).
		[ExportGroup("Gameplay")]
		[Export] private int _scoreBaseValue = 300;
		[Export] private int _damageToPlayer = 20;
		[Export] private int _damageToPlanet = 10;

		// Internal state for movement behavior.
		private float _rotationSpeed;
		private Vector2 _center;

		// Convenience proxy — always reflects the current HealthComponent state.
		private bool IsDead => _health.IsDead;
		private void ForceKill() => _health.Kill();

		// IDamageable — delegated to HealthComponent.
		public void ApplyDamage(int damage) => _health.ApplyDamage(damage);

		// External hook — used by difficulty scaling systems.
		public void AddSpeed(float additionalSpeed) => _speed.AddSpeed(additionalSpeed);

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
			_animatedSprite.AnimationFinished += OnAnimationFinished;
			_health.Death += OnDeath;
			BodyEntered += OnBodyEntered;
			AreaEntered += OnAreaEntered;
		}

		private void DisconnectSignals()
		{
			_animatedSprite.AnimationFinished -= OnAnimationFinished;
			_health.Death -= OnDeath;
			BodyEntered -= OnBodyEntered;
			AreaEntered -= OnAreaEntered;
		}

		private void OnBodyEntered(Node2D entity)
		{
			// Handles collisions with physics bodies (e.g., player).
			if (entity is IDamageable damageable)
				damageable.ApplyDamage(_damageToPlayer);

			ForceKill();
		}

		private void OnAreaEntered(Area2D entity)
		{
			// Ignore other asteroids to prevent chain-reaction collisions.
			if (entity.IsInGroup("Asteroid")) return;

			// Handles interactions with non-body entities (e.g., planet hitboxes).
			if (entity is IDamageable damageable)
				damageable.ApplyDamage(_damageToPlanet);

			ForceKill();
		}

		private void OnDeath()
		{
			// Disable collision immediately to prevent further interactions during death animation.
			_collision.SetDeferred("disabled", true);

			// Trigger explosion animation and notify scoring systems.
			_animatedSprite.Play("explode");
			EmitSignal(SignalName.AsteroidDestroyed, _scoreBaseValue);
		}

		private void OnAnimationFinished()
		{
			// Cleanup only after explosion animation completes.
			if (_animatedSprite.Animation != "explode") return;
			QueueFree();
		}

		// ------------------------------------ Movement logic --------------------------------------

		private void MoveTowardsCenter(double delta)
		{
			// Moves asteroid towards screen center with slight rotational drift.
			var direction = (_center - GlobalPosition).Normalized();
			GlobalPosition += direction * _speed.CurrentSpeed * (float)delta;
			Rotation += _rotationSpeed;
		}
	}
}