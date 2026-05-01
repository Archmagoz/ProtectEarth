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

		// Runtime state — cached initial speed for reset operations.
		private float _defaultSpeed;

		// ------------------------------------- Godot overrides ------------------------------------

		public override void _Ready()
		{
			// Captures initial speed as baseline for future resets.
			_defaultSpeed = CurrentSpeed;
		}

		// ---------------------------------------- Public API --------------------------------------

		public void Reset()
		{
			// Restores speed to its initial configured value.
			UpdateSpeed(_defaultSpeed);
		}

		public void SetSpeed(float value)
		{
			// Directly overrides current speed (clamped internally).
			UpdateSpeed(value);
		}

		public void AddSpeed(float amount)
		{
			// Increases speed relative to current value.
			UpdateSpeed(CurrentSpeed + amount);
		}

		public void ReduceSpeed(float amount)
		{
			// Decreases speed relative to current value.
			UpdateSpeed(CurrentSpeed - amount);
		}

		// ----------------------------------------- Helpers ----------------------------------------

		private void UpdateSpeed(float value)
		{
			// Centralized speed mutation logic with clamping and change detection.
			var oldSpeed = CurrentSpeed;

			CurrentSpeed = Mathf.Clamp(value, 0, MaxSpeed);

			if (!Mathf.IsEqualApprox(CurrentSpeed, oldSpeed))
				EmitSignal(SignalName.SpeedChanged, CurrentSpeed, MaxSpeed);
		}
	}
}