using ProtectEarth.Entities.Projectile;

using Godot;

namespace ProtectEarth.Entities
{
	public partial class Player : CharacterBody2D
	{
		// Node references (assigned via editor or auto-resolved in _Ready).
		[Export] public Components.HealthComponent Health { get; private set; }
		[Export] public Components.SpeedComponent Speed { get; private set; }
		[Export] public CollisionPolygon2D Collision { get; private set; }
		[Export] public PackedScene ProjectileScene { get; private set; }
		[Export] public Marker2D Muzzle { get; private set; }

		private Vector2 _velocity;

		public override void _Ready()
		{
			// Fallback to find nodes if not set via editor.
			Health ??= GetNodeOrNull<Components.HealthComponent>("HealthComponent");
			Speed ??= GetNodeOrNull<Components.SpeedComponent>("SpeedComponent");
			Collision ??= GetNodeOrNull<CollisionPolygon2D>("Collision");
			Muzzle ??= GetNode<Marker2D>("Muzzle");

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
			if (Input.IsActionJustPressed("shoot"))
			{
				Shoot();
			}
		}

		// ------------------------------ Signal handlers ----------------------------------

		private void OnDeath()
		{
			return; // Placeholder for death logic.
		}

		// ------------------------------ Player actions -----------------------------------

		private void Shoot()
		{
			var projectile = ProjectileScene.Instantiate<PlayerProjectile>();
			var direction = (GetGlobalMousePosition() - GlobalPosition).Normalized();

			projectile.GlobalPosition = Muzzle.GlobalPosition;
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
			Vector2 mousePos = GetGlobalMousePosition();
			LookAt(mousePos);
		}
	}
}
