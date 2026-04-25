using Godot;

namespace ProtectEarth.Components
{
	[GlobalClass]
	public partial class HealthComponent : Node
	{
		[Export] public int MaxHealth { get; set; } = 100;

		[Signal] public delegate void HealthChangedEventHandler(int current, int max);
		[Signal] public delegate void DeathEventHandler();

		public bool IsDead { get; private set; }
		public int CurrentHealth { get; private set; }

		public override void _Ready()
		{
			CurrentHealth = MaxHealth;
		}

		// Internal method to update health safely.
		private void UpdateHealth(int value)
		{
			int oldHealth = CurrentHealth;
			CurrentHealth = Mathf.Clamp(value, 0, MaxHealth);

			if (CurrentHealth != oldHealth)
				EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);

			if (CurrentHealth == 0 && !IsDead)
			{
				IsDead = true;
				EmitSignal(SignalName.Death);
			}
		}

		// Public API
		public void Reset()
		{
			IsDead = false;
			UpdateHealth(MaxHealth);
		}

		public void SetHealth(int value)
		{
			UpdateHealth(value);
		}

		public void ApplyDamage(int damage)
		{
			if (IsDead) return;
			UpdateHealth(CurrentHealth - damage);
		}

		public void Heal(int amount)
		{
			if (IsDead) return;
			UpdateHealth(CurrentHealth + amount);
		}
	}
}
