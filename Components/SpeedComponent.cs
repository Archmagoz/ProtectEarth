using Godot;
using ProtectEarth.Utils;

namespace ProtectEarth.Components
{
	[GlobalClass]
	public partial class SpeedComponent : Node
	{
		[Export] public float MaxSpeed { get; set; } = 100f;

		[Signal] public delegate void SpeedChangedEventHandler(float current, float max);

		private float _currentSpeed;
		public float CurrentSpeed => _currentSpeed;

		public override void _Ready()
		{
			float speed = MaxSpeed;
			_currentSpeed = speed;
		}

		// Internal method to update speed safely.
		private void UpdateSpeed(float value)
		{
			float oldSpeed = _currentSpeed;
			_currentSpeed = Mathf.Clamp(value, 0, MaxSpeed);

			if (!Mathf.IsEqualApprox(_currentSpeed, oldSpeed))
				EmitSignal(SignalName.SpeedChanged, _currentSpeed, MaxSpeed);
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
			UpdateSpeed(_currentSpeed + amount);
		}

		public void ReduceSpeed(float amount)
		{
			UpdateSpeed(_currentSpeed - amount);
		}
	}
}
