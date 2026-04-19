using Godot;

namespace ProtectEarth.Components
{
	public partial class HealthComponent : Node
	{
		[Export] public int MaxHealth { get; set; }

		private int _currentHealth;
		public int CurrentHealth => _currentHealth;

		private bool _isDead = false;
		public bool IsDead => _isDead;

		[Signal] public delegate void HealthChangedEventHandler(int current, int max);
		[Signal] public delegate void DeathEventHandler();

		public override void _Ready()
		{
			_currentHealth = MaxHealth;
		}

		private void UpdateHealth(int value)
		{
			int oldHealth = _currentHealth;
			_currentHealth = Mathf.Clamp(value, 0, MaxHealth);

			if (_currentHealth != oldHealth)
				EmitSignal(SignalName.HealthChanged, _currentHealth, MaxHealth);

			if (_currentHealth == 0 && !_isDead)
			{
				_isDead = true;
				EmitSignal(SignalName.Death);
			}
		}

		public void Reset()
		{
			_isDead = false;
			UpdateHealth(MaxHealth);
		}

		public void ApplyDamage(int damage)
		{
			if (IsDead) return;
			UpdateHealth(_currentHealth - damage);
		}


		public void Heal(int amount)
		{
			if (IsDead) return;
			UpdateHealth(_currentHealth + amount);
		}
	}

}
