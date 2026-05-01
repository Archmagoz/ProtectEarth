using Godot;

namespace ProtectEarth.Components
{
	[GlobalClass]
	public partial class SpeedComponent : Node
	{
		// Signal Handler.
		[Signal] public delegate void SpeedChangedEventHandler(float current, float max);

		// Base values (assigned via editor).
		[Export] public float CurrentSpeed { get; private set; } = 10f;
		[Export] public float MaxSpeed { get; private set; } = 100f;

		private float _defaultSpeed;

		// ------------------------------ Godot overrides ----------------------------------

		public override void _Ready() => _defaultSpeed = CurrentSpeed;

		// ------------------------------ Public API ----------------------------------

		public void Reset() => UpdateSpeed(_defaultSpeed);

		public void SetSpeed(float value) => UpdateSpeed(value);

		public void AddSpeed(float amount) => UpdateSpeed(CurrentSpeed + amount);

		public void ReduceSpeed(float amount) => UpdateSpeed(CurrentSpeed - amount);

		// ------------------------------ Helpers ----------------------------------

		private void UpdateSpeed(float value)
		{
			var oldSpeed = CurrentSpeed;
			CurrentSpeed = Mathf.Clamp(value, 0, MaxSpeed);

			if (!Mathf.IsEqualApprox(CurrentSpeed, oldSpeed))
				EmitSignal(SignalName.SpeedChanged, CurrentSpeed, MaxSpeed);
		}
	}
}