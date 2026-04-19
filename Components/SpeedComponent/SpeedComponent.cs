using Godot;

namespace ProtectEarth.Components
{
	[GlobalClass]
	public partial class SpeedComponent : Node
	{
		[Export] public float MaxSpeed { get; set; }

		private float _currentSpeed;
		public float CurrentSpeed => _currentSpeed;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			_currentSpeed = MaxSpeed / 2;
		}
	}
}
