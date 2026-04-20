using Godot;
using ProtectEarth.Components;

namespace ProtectEarth.Entities
{
	public partial class Planet : Node2D
	{
		[Export] public HealthComponent Health;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Health ??= GetNodeOrNull<HealthComponent>("HealthComponent");
		}
	}
}
