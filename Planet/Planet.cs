using Godot;

namespace ProtectEarth.Objects
{
	public partial class Planet : Node2D
	{
		[Export] public Components.HealthComponent Health;

		public override void _Ready()
		{
		}

		public override void _Process(double delta)
		{
		}
	}
}
