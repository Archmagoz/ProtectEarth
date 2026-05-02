using ProtectEarth.Core.Interfaces;
using ProtectEarth.Components;

using Godot;

namespace ProtectEarth.Entities
{
	public partial class Player : CharacterBody2D, IDamageable
	{
		// Signal Handler.
		[Signal] public delegate void PlayerDiedEventHandler();

		// Node references (assigned via editor).
		[ExportGroup("Components")]
		[Export] private CollisionPolygon2D _collision;
		[Export] private Marker2D _marker;
		[Export] private HealthComponent _health;
		[Export] private SpeedComponent _speed;

		// Shooting behaviour parameters (assigned via editor).
		[ExportGroup("Gameplay")]
		[Export] private PackedScene _projectileScene;
		[Export] private float _shootBufferTime = 0.15f;
		[Export] private float _fireRate = 0.3f;

		// Internal state for shooting cooldown and input buffering.
		private float _shootCooldown = 0f;
		private float _shootBuffer = 0f;

		// Convenience proxy — always reflects the current HealthComponent state.
		private bool IsDead => _health.IsDead;

		// IDamageable — delegated to HealthComponent.
		public void ApplyDamage(int damage) => _health.ApplyDamage(damage);

		// ------------------------------------- Godot overrides ------------------------------------

		public override void _Ready()
		{
			AddToGroup("ProjectileImmune");
			ConnectSignals();
		}

		public override void _PhysicsProcess(double delta)
		{
			if (IsDead) return;
			HandleMovement();
			HandleRotation();
		}

		public override void _Process(double delta)
		{
			if (IsDead) return;
			HandleShooting(delta);
		}

		public override void _ExitTree() => DisconnectSignals();

		// ------------------------------------ Signal management -----------------------------------

		private void ConnectSignals() => _health.Death += OnDeath;

		private void DisconnectSignals() => _health.Death -= OnDeath;

		private void OnDeath() => CallDeferred(nameof(HandleDeath));

		private void HandleDeath()
		{
			_collision.Disabled = true;
			EmitSignal(SignalName.PlayerDied);
		}

		// ------------------------------------- Player actions -------------------------------------

		private void HandleShooting(double delta)
		{
			var d = (float)delta;

			_shootCooldown -= d;
			_shootBuffer -= d;

			if (Input.IsActionJustPressed("shoot")) _shootBuffer = _shootBufferTime;
			if (_shootCooldown > 0f) return;
			if (_shootBuffer <= 0f && !Input.IsActionPressed("shoot")) return;

			Shoot();

			_shootCooldown = _fireRate;
			_shootBuffer = 0f;
		}

		private void Shoot()
		{
			var projectile = _projectileScene.Instantiate<Projectile>();
			var direction = (GetGlobalMousePosition() - GlobalPosition).Normalized();

			projectile.Rotation = direction.Angle() + Mathf.Pi / 2f;
			projectile.GlobalPosition = _marker.GlobalPosition;
			projectile.Direction = direction;

			GetParent().AddChild(projectile);
		}

		private void HandleMovement()
		{
			var input = Vector2.Zero;

			if (Input.IsActionPressed("up")) input.Y -= 1;
			if (Input.IsActionPressed("down")) input.Y += 1;
			if (Input.IsActionPressed("left")) input.X -= 1;
			if (Input.IsActionPressed("right")) input.X += 1;

			Velocity = input.Normalized() * _speed.CurrentSpeed;
			MoveAndSlide();
		}

		private void HandleRotation() => LookAt(GetGlobalMousePosition());
	}
}