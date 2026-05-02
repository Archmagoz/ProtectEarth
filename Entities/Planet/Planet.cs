using ProtectEarth.Core.Interfaces;
using ProtectEarth.Components;

using Godot;

namespace ProtectEarth.Entities
{
	public partial class Planet : StaticBody2D, IDamageable
	{
		// Signal Handler.
		[Signal] public delegate void PlanetDestroyedEventHandler();

		// Node references (assigned via editor).
		[ExportGroup("Components")]
		[Export] private CollisionShape2D _collision;
		[Export] private HealthComponent _health;

		// Convenience proxy — always reflects the current HealthComponent state.
		private bool IsDead => _health.IsDead;

		// IDamageable — delegated to HealthComponent.
		public void ApplyDamage(int damage) => _health.ApplyDamage(damage);

		// ------------------------------------- Godot overrides ------------------------------------

		public override void _Ready()
		{
			// Prevents projectile interactions while still allowing damage via other systems.
			AddToGroup("ProjectileImmune");

			ConnectSignals();
		}

		public override void _ExitTree() => DisconnectSignals();

		// ------------------------------------ Signal management -----------------------------------

		private void ConnectSignals()
		{
			_health.Death += OnDeath;
		}

		private void DisconnectSignals()
		{
			_health.Death -= OnDeath;
		}

		// ------------------------------------ Signal handlers -------------------------------------

		private void OnDeath()
		{
			// Emits destruction event for game state systems (e.g., game over).
			EmitSignal(SignalName.PlanetDestroyed);
		}
	}
}