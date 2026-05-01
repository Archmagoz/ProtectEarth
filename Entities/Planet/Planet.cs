using ProtectEarth.Core.Interfaces;
using ProtectEarth.Core.Utils;
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
		[Export] public CollisionShape2D Collision { get; private set; }
		[Export] public HealthComponent Health { get; private set; }

		// Convenience proxy — always reflects the current HealthComponent state.
		private bool IsDead => Health.IsDead;

		// IDamageable — delegated to HealthComponent.
		public void ApplyDamage(int damage) => Health.ApplyDamage(damage);

		// ------------------------------------- Godot overrides ------------------------------------

		public override void _Ready()
		{
			this.ValidateExports();

			// Prevents projectile interactions while still allowing damage via other systems.
			AddToGroup("ProjectileImmune");

			ConnectSignals();
		}

		public override void _ExitTree() => DisconnectSignals();

		// ------------------------------------ Signal management -----------------------------------

		private void ConnectSignals()
		{
			Health.Death += OnDeath;
		}

		private void DisconnectSignals()
		{
			Health.Death -= OnDeath;
		}

		// ------------------------------------ Signal handlers -------------------------------------

		private void OnDeath()
		{
			// Emits destruction event for game state systems (e.g., game over).
			EmitSignal(SignalName.PlanetDestroyed);
		}
	}
}