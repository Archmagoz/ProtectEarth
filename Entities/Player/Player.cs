using ProtectEarth.Core.Interfaces;
using ProtectEarth.Entities.Projectile;

using Godot;

namespace ProtectEarth.Entities
{
	public partial class Player : CharacterBody2D, IDamageable
	{
		// Node references (assigned via editor or auto-resolved in _Ready).
		[Export] public Components.HealthComponent Health { get; private set; }
		[Export] public Components.SpeedComponent Speed { get; private set; }
		[Export] public CollisionPolygon2D Collision { get; private set; }
		[Export] public PackedScene ProjectileScene { get; private set; }
		[Export] public Marker2D Marker { get; private set; }

		// Player firing control.
		[Export] public float ShootBufferTime = 0.15f;
		[Export] public float FireRate = 0.3f;
		private float _shootCooldown = 0f;
		private float _shootBuffer = 0f;

		// IDamageable delegate to HealthComponent.
		public void ApplyDamage(int damage) => Health.ApplyDamage(damage);

		public override void _Ready()
		{
			// Fallback to find nodes if not set via editor.
			Health ??= GetNodeOrNull<Components.HealthComponent>("HealthComponent");
			Speed ??= GetNodeOrNull<Components.SpeedComponent>("SpeedComponent");
			Collision ??= GetNodeOrNull<CollisionPolygon2D>("Collision");
			Marker ??= GetNode<Marker2D>("Marker");

			// Connect signals.
			Health.Death += OnDeath;
		}

		public override void _PhysicsProcess(double delta)
		{
			if (Health.IsDead) return; // early exit if dead
			HandleMovement();
			HandleRotation();
		}

		public override void _Process(double delta)
		{
			if (Health.IsDead) return; // early exit if dead
			HandleShooting(delta);
		}

		// ------------------------------ Signal handlers ----------------------------------

		private void OnDeath()
		{
			return; // Placeholder for death logic.
		}

		// ------------------------------ Player actions -----------------------------------

		// Handles shooting input, manages cooldowns and buffers, and calls Shoot() when appropriate.
		private void HandleShooting(double delta)
		{
			float d = (float)delta;

			// Update cooldowns and buffers.
			_shootCooldown -= d;
			_shootBuffer -= d;

			// Check for shoot input and manage shooting logic.
			if (Input.IsActionJustPressed("shoot"))
			{
				_shootBuffer = ShootBufferTime;
			}

			if (_shootCooldown > 0f) return;

			if (_shootBuffer <= 0f && !Input.IsActionPressed("shoot")) return;

			Shoot();

			// Reset cooldown and buffer after shooting.
			_shootCooldown = FireRate;
			_shootBuffer = 0f;
		}

		// Instantiates a projectile, sets its direction and source, and adds it to the scene.
		private void Shoot()
		{
			var projectile = ProjectileScene.Instantiate<PlayerProjectile>();
			var direction = (GetGlobalMousePosition() - GlobalPosition).Normalized();

			projectile.GlobalPosition = Marker.GlobalPosition;
			projectile.Rotation = Rotation + Mathf.Pi / 2f;
			projectile.Direction = direction;
			projectile.Source = this;

			GetTree().CurrentScene.AddChild(projectile);
		}

		// ------------------------------ Movement logic ----------------------------------

		private void HandleMovement()
		{
			Vector2 input = Vector2.Zero;

			if (Input.IsActionPressed("up")) input.Y -= 1;
			if (Input.IsActionPressed("down")) input.Y += 1;
			if (Input.IsActionPressed("left")) input.X -= 1;
			if (Input.IsActionPressed("right")) input.X += 1;

			input = input.Normalized();

			Velocity = input * Speed.CurrentSpeed;

			MoveAndSlide();
		}

		private void HandleRotation()
		{
			var mousePos = GetGlobalMousePosition();
			LookAt(mousePos);
		}
	}
}
