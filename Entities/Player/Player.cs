using ProtectEarth.Core.Interfaces;
using ProtectEarth.Components;

using Godot;

namespace ProtectEarth.Entities
{
	public partial class Player : CharacterBody2D, IDamageable
	{
		// Node references (assigned via editor).
		[ExportGroup("Components")]
		[Export] public CollisionPolygon2D Collision { get; private set; }
		[Export] public Marker2D Marker { get; private set; }
		[Export] public HealthComponent Health { get; private set; }
		[Export] public SpeedComponent Speed { get; private set; }

		// Shooting behaviour parameters (assigned via editor).
		[ExportGroup("Gameplay")]
		[Export] public PackedScene ProjectileScene { get; private set; }
		[Export] public float ShootBufferTime = 0.15f;
		[Export] public float FireRate = 0.3f;

		// Internal state for shooting cooldown and input buffering.
		private float _shootCooldown = 0f;
		private float _shootBuffer = 0f;

		// Convenience proxy — always reflects the current HealthComponent state.
		private bool IsDead => Health.IsDead;

		// IDamageable — delegated to HealthComponent.
		public void ApplyDamage(int damage) => Health.ApplyDamage(damage);

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
		}

		public override void _Process(double delta)
		{
			if (IsDead) return;
			HandleRotation();
			HandleShooting(delta);
		}

		public override void _ExitTree() => DisconnectSignals();

		// ------------------------------------ Signal management -----------------------------------

		private void ConnectSignals() => Health.Death += OnDeath;

		private void DisconnectSignals() => Health.Death -= OnDeath;

		// ------------------------------------ Signal handlers -------------------------------------

		private void OnDeath()
		{
			// TODO: death state logic.
		}

		// ------------------------------------- Player actions -------------------------------------

		private void HandleShooting(double delta)
		{
			var d = (float)delta;

			_shootCooldown -= d;
			_shootBuffer -= d;

			if (Input.IsActionJustPressed("shoot")) _shootBuffer = ShootBufferTime;
			if (_shootCooldown > 0f) return;
			if (_shootBuffer <= 0f && !Input.IsActionPressed("shoot")) return;

			Shoot();

			_shootCooldown = FireRate;
			_shootBuffer = 0f;
		}

		private void Shoot()
		{
			var projectile = ProjectileScene.Instantiate<Projectile>();
			var direction = (GetGlobalMousePosition() - GlobalPosition).Normalized();

			projectile.Rotation = direction.Angle() + Mathf.Pi / 2f;
			projectile.GlobalPosition = Marker.GlobalPosition;
			projectile.Direction = direction;

			GetParent().AddChild(projectile);
		}

		// ------------------------------------ Movement logic --------------------------------------

		private void HandleMovement()
		{
			var input = Vector2.Zero;

			if (Input.IsActionPressed("up")) input.Y -= 1;
			if (Input.IsActionPressed("down")) input.Y += 1;
			if (Input.IsActionPressed("left")) input.X -= 1;
			if (Input.IsActionPressed("right")) input.X += 1;

			Velocity = input.Normalized() * Speed.CurrentSpeed;
			MoveAndSlide();
		}

		private void HandleRotation() => LookAt(GetGlobalMousePosition());
	}
}