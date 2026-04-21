using Godot;
using ProtectEarth.Components;

namespace ProtectEarth.Entities
{
	[GlobalClass]
	public partial class Projectile : Area2D
	{
		// Node references (assigned via editor or auto-resolved in _Ready).
		[Export] public AnimatedSprite2D AnimatedSprite { get; private set; }
		[Export] public CollisionShape2D Collision { get; private set; }
		[Export] public SpeedComponent Speed { get; private set; }

		public override void _Ready()
		{
			// Fallback to find nodes if not set via editor.
			AnimatedSprite ??= GetNodeOrNull<AnimatedSprite2D>("Sprite");
			Collision ??= GetNodeOrNull<CollisionShape2D>("Collision");
			Speed ??= GetNodeOrNull<SpeedComponent>("SpeedComponent");
		}

		public override void _PhysicsProcess(double delta)
		{
		}
	}
}
