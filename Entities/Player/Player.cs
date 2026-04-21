using Godot;

namespace ProtectEarth.Entities
{
	public partial class Player : CharacterBody2D
	{
		// Node references (assigned via editor or auto-resolved in _Ready).
		[Export] public Components.HealthComponent Health { get; private set; }
		[Export] public Components.SpeedComponent Speed { get; private set; }
		[Export] public CollisionPolygon2D Collision { get; private set; }

		private Vector2 _velocity;
		private bool _isDead = false;

		public bool IsDead => _isDead;

		public override void _Ready()
		{
			// Fallback to find nodes if not set via editor.
			Health ??= GetNodeOrNull<Components.HealthComponent>("HealthComponent");
			Speed ??= GetNodeOrNull<Components.SpeedComponent>("SpeedComponent");
			Collision ??= GetNodeOrNull<CollisionPolygon2D>("Collision");

			// Connect signals.
			Health.Death += OnDeath;
		}

		public override void _PhysicsProcess(double delta)
		{
			if (_isDead) return;
			HandleMovement();
			HandleRotation();
		}

		// ------------------------------ Signal handlers ----------------------------------

		private void OnDeath()
		{
			_isDead = true;
			return; // Placeholder for death logic.
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
