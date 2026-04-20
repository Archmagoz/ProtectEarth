using Godot;
using ProtectEarth.Components;

namespace ProtectEarth.Entities
{
	public partial class Planet : Node2D
	{
		// References (set via editor or fallback in _Ready).
		[Export] public HealthComponent Health;

		public override void _Ready()
		{
			// Fallback to find nodes if not set via editor.
			Health ??= GetNodeOrNull<HealthComponent>("HealthComponent");
		}
	}
}
