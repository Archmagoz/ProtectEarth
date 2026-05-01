using ProtectEarth.Core.Interfaces;
using ProtectEarth.Components;

using Godot;
using System;

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

		// Gameplay parameters for shooting behaviour (assigned via editor).
		[ExportGroup("Gameplay")]
		[Export] public PackedScene ProjectileScene { get; private set; }
		[Export] public float ShootBufferTime = 0.15f;
		[Export] public float FireRate = 0.3f;

		// Internal state for shooting cooldowns and buffers.
		private float _shootCooldown = 0f;
		private float _shootBuffer = 0f;

		// Convenience proxy — always reflects the current HealthComponent state.
		private bool IsDead => Health.IsDead;

		// IDamageable delegate to HealthComponent.
		public void ApplyDamage(int damage) => Health.ApplyDamage(damage);

		// ------------------------------ Godot overrides ----------------------------------

		public override void _Ready()
		{
			// Hard validation — these components are required for the Player to function.
			// Throws in all build configurations, ensuring misconfigured scenes are caught early.
			if (Collision == null)
				throw new InvalidOperationException("Collision is not assigned on Player.");
			if (Marker == null)
				throw new InvalidOperationException("Marker is not assigned on Player.");
			if (Health == null)
				throw new InvalidOperationException("HealthComponent is not assigned on Player.");
			if (Speed == null)
				throw new InvalidOperationException("SpeedComponent is not assigned on Player.");
			if (ProjectileScene == null)
				throw new InvalidOperationException("ProjectileScene is not assigned on Player.");

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

		public override void _ExitTree()
		{
			DisconnectSignals();
		}

		// ------------------------------ Signal management ----------------------------------

		private void ConnectSignals()
		{
			Health.Death += OnDeath;
		}

		private void DisconnectSignals()
		{
			Health.Death -= OnDeath;
		}

		// ------------------------------ Signal handlers ----------------------------------

		private void OnDeath()
		{
			// TODO: OnDeath state logic.
		}

		// ------------------------------ Player actions -----------------------------------

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

		// ------------------------------ Movement logic ----------------------------------

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