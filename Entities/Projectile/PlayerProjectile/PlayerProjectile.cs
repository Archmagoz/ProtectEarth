using ProtectEarth.Core.Interfaces;
using ProtectEarth.Components;

using Godot;

namespace ProtectEarth.Entities.Projectile
{
	[GlobalClass]
	public partial class PlayerProjectile : Area2D
	{
		// Node references (assigned via editor or auto-resolved in _Ready).
		[Export] public AnimatedSprite2D AnimatedSprite { get; private set; }
		[Export] public CollisionShape2D Collision { get; private set; }
		[Export] public SpeedComponent Speed { get; private set; }

		// Source reference to avoid self-collision.
		public Node Source { get; set; }
		public Vector2 Direction { get; set; }

		public override void _Ready()
		{
			// Fallback to find nodes if not set via editor.
			AnimatedSprite ??= GetNodeOrNull<AnimatedSprite2D>("Sprite");
			Collision ??= GetNodeOrNull<CollisionShape2D>("Collision");
			Speed ??= GetNodeOrNull<SpeedComponent>("SpeedComponent");

			// Connect signals.
			BodyEntered += OnBodyEntered;
		}

		public override void _PhysicsProcess(double delta)
		{
			Position += Direction * Speed.CurrentSpeed * (float)delta;
		}

		// Checks for collisions with damageable entities, applies damage and frees the projectile.
		private void OnBodyEntered(Node body)
		{
			if (body == Source) return; // Ignore collisions with the source (player)

			if (body is IDamageable damageable)
			{
				damageable.ApplyDamage(100);
				QueueFree();
			}
		}

		// Free the projectile when Lifetime Timer ends, timer set by godot editor.
		private void OnLifetimeTimeout() => QueueFree();
	}
}
