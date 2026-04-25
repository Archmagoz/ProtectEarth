using Godot;

namespace ProtectEarth.Components
{
	[GlobalClass]
	public partial class SpeedComponent : Node
	{
		[Export] public float MaxSpeed { get; set; } = 100f;

		[Signal] public delegate void SpeedChangedEventHandler(float current, float max);

		public float CurrentSpeed { get; private set; }

		public override void _Ready()
		{
			float speed = MaxSpeed;
			CurrentSpeed = speed;
		}

		// Internal method to update speed safely.
		private void UpdateSpeed(float value)
		{
			float oldSpeed = CurrentSpeed;
			CurrentSpeed = Mathf.Clamp(value, 0, MaxSpeed);

			if (!Mathf.IsEqualApprox(CurrentSpeed, oldSpeed))
				EmitSignal(SignalName.SpeedChanged, CurrentSpeed, MaxSpeed);
		}

		// Public API
		public void Reset()
		{
			UpdateSpeed(MaxSpeed);
		}

		public void SetSpeed(float value)
		{
			UpdateSpeed(value);
		}

		public void AddSpeed(float amount)
		{
			UpdateSpeed(CurrentSpeed + amount);
		}

		public void ReduceSpeed(float amount)
		{
			UpdateSpeed(CurrentSpeed - amount);
		}
	}
}
