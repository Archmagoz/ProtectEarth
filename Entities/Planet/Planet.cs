using Godot;
using ProtectEarth.Components;

namespace ProtectEarth.Entities
{
	public partial class Planet : Node2D
	{
		// Node references (assigned via editor or auto-resolved in _Ready).
		[Export] public HealthComponent Health;

		public override void _Ready()
		{
			// Fallback to find nodes if not set via editor.
			Health ??= GetNodeOrNull<HealthComponent>("HealthComponent");
		}
	}
}
