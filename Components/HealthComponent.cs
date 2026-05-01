using Godot;

namespace ProtectEarth.Components
{
	[GlobalClass]
	public partial class HealthComponent : Node
	{
		// Signal Handlers.
		[Signal] public delegate void HealthChangedEventHandler(int current, int max);
		[Signal] public delegate void DeathEventHandler();

		// Base value (assigned via editor).
		[Export] public int MaxHealth { get; set; } = 100;

		// Runtime state — managed internally by the component.
		public bool IsDead { get; private set; }
		public int CurrentHealth { get; private set; }

		// ------------------------------------- Godot overrides ------------------------------------

		public override void _Ready()
		{
			// Initializes health state to its maximum value on spawn.
			CurrentHealth = MaxHealth;
		}

		// ---------------------------------------- Public API --------------------------------------

		public void Reset()
		{
			// Restores entity to a fully alive state.
			IsDead = false;
			UpdateHealth(MaxHealth);
		}

		public void SetHealth(int value)
		{
			// Directly overrides current health value (clamped internally).
			UpdateHealth(value);
		}

		public void ApplyDamage(int damage)
		{
			// Applies damage only if the entity is still alive.
			if (IsDead) return;
			UpdateHealth(CurrentHealth - damage);
		}

		public void Heal(int amount)
		{
			// Applies healing only if the entity is still alive.
			if (IsDead) return;
			UpdateHealth(CurrentHealth + amount);
		}

		public void Kill()
		{
			// Forces death state and emits termination signal once.
			if (IsDead) return;
			IsDead = true;
			EmitSignal(SignalName.Death);
		}

		// ----------------------------------------- Helpers ----------------------------------------

		private void UpdateHealth(int value)
		{
			// Centralized health mutation logic with clamping and event dispatching.
			var oldHealth = CurrentHealth;

			CurrentHealth = Mathf.Clamp(value, 0, MaxHealth);

			if (CurrentHealth != oldHealth)
				EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);

			if (CurrentHealth <= 0)
				Kill();
		}
	}
}