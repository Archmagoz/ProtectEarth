using Godot;
using ProtectEarth.Components;

namespace ProtectEarth.Entities
{
	public partial class Planet : Node2D
	{
		[Export] public HealthComponent Health;

		public override void _Ready()
		{
			Health ??= GetNodeOrNull<HealthComponent>("HealthComponent");
		}
	}
}
