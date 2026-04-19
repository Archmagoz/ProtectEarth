using Godot;

namespace ProtectEarth.Entities
{
	using ProtectEarth.Components;

	public partial class Planet : Node2D
	{
		[Export] public HealthComponent Health;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Health ??= GetNodeOrNull<HealthComponent>("HealthComponent");
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}
	}
}
