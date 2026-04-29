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
		[ExportGroup("Components")]
		[Export] public AnimatedSprite2D AnimatedSprite { get; private set; }
		[Export] public CollisionPolygon2D Collision { get; private set; }
		[Export] public HealthComponent Health { get; private set; }
		[Export] public SpeedComponent Speed { get; private set; }

		// Gameplay parameters for scoring and player damage.
		[ExportGroup("Gameplay")]
		[Export] public int ScoreBaseValue { get; private set; } = 300;
		[Export] public int DamageToPlayer { get; private set; } = 20;

		// Internal state for rotation and screen center caching.
		private float _rotationSpeed;
		private Vector2 _center;

		// IDamageable delegate to HealthComponent.
		public void ApplyDamage(int damage) => Health.ApplyDamage(damage);

		// Delegate AddSpeed to SpeedComponent, used by GameManager on difficulty changes.
		public void AddSpeed(float additionalSpeed) => Speed.AddSpeed(additionalSpeed);

		// ------------------------------ Godot overrides ----------------------------------

		public override void _Ready()
		{
			AnimatedSprite ??= GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite");
			Collision ??= GetNodeOrNull<CollisionPolygon2D>("Collision");
			Health ??= GetNodeOrNull<HealthComponent>("HealthComponent");
			Speed ??= GetNodeOrNull<SpeedComponent>("SpeedComponent");

			_center = ScreenUtils.GetScreenCenter(this);
			_rotationSpeed = RNG.Range(-0.01f, 0.01f);

			ConnectSignals();
		}

		public override void _PhysicsProcess(double delta)
		{
			if (Health.IsDead) return;
			MoveTowardsCenter();
		}

		public override void _ExitTree()
		{
			DisconnectSignals();
		}

		// ------------------------------ Signal management ----------------------------------

		private void ConnectSignals()
		{
			AnimatedSprite.AnimationFinished += OnAnimationFinished;
			Health.Death += OnDeath;
		}

		private void DisconnectSignals()
		{
			AnimatedSprite.AnimationFinished -= OnAnimationFinished;
			Health.Death -= OnDeath;
		}

		// ------------------------------ Signal handlers ----------------------------------

		private void OnDeath()
		{
			LinearVelocity = Vector2.Zero;
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

		private void MoveTowardsCenter()
		{
			var direction = (_center - GlobalPosition).Normalized();
			LinearVelocity = direction * Speed.CurrentSpeed;
			Rotation += _rotationSpeed;
		}
	}
}