using Godot;
using ProtectEarth.Components;

namespace ProtectEarth.Entities
{
	public partial class Planet : Node2D
	{
		// Node references (assigned via editor or auto-resolved in _Ready).
		[Export] public HealthComponent Health { get; private set; }
		[Export] public CollisionShape2D Collision { get; private set; }

		public override void _Ready()
		{
			// Fallback to find nodes if not set via editor.
			Health ??= GetNodeOrNull<HealthComponent>("HealthComponent");
			Collision ??= GetNodeOrNull<CollisionShape2D>("Collision");
		}
	}
}
