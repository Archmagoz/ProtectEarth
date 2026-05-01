using Godot;

namespace ProtectEarth.Components
{
	[GlobalClass]
	public partial class HealthComponent : Node
	{
		// Signals Handlers.
		[Signal] public delegate void HealthChangedEventHandler(int current, int max);
		[Signal] public delegate void DeathEventHandler();

		// Base value (assigned via editor).
		[Export] public int MaxHealth { get; set; } = 100;

		public bool IsDead { get; private set; }
		public int CurrentHealth { get; private set; }

		// ------------------------------ Godot overrides ----------------------------------

		public override void _Ready() => CurrentHealth = MaxHealth;

		// ------------------------------ Public API ----------------------------------

		public void Reset()
		{
			IsDead = false;
			UpdateHealth(MaxHealth);
		}

		public void SetHealth(int value) => UpdateHealth(value);

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

		public void Kill()
		{
			if (IsDead) return;
			IsDead = true;
			EmitSignal(SignalName.Death);
		}

		// ------------------------------ Helpers ----------------------------------

		private void UpdateHealth(int value)
		{
			var oldHealth = CurrentHealth;

			CurrentHealth = Mathf.Clamp(value, 0, MaxHealth);

			if (CurrentHealth != oldHealth)
				EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);

			if (CurrentHealth <= 0)
				Kill();
		}
	}
}