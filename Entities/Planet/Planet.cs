using ProtectEarth.Core.Controllers;
using ProtectEarth.Core.Interfaces;
using ProtectEarth.Components;

using Godot;
using System;

namespace ProtectEarth.Entities
{
	public partial class Planet : StaticBody2D, IDamageable
	{
		// Node references (assigned via editor).
		[ExportGroup("Components")]
		[Export] public CollisionShape2D Collision { get; private set; }
		[Export] public HealthComponent Health { get; private set; }

		// Convenience proxy — always reflects the current HealthComponent state.
		private bool IsDead => Health.IsDead;

		// IDamageable delegate to HealthComponent.
		public void ApplyDamage(int damage) => Health.ApplyDamage(damage);

		// ------------------------------ Godot overrides ----------------------------------

		public override void _Ready()
		{
			// Hard validation — these components are required for the Planet to function.
			// Throws in all build configurations, ensuring misconfigured scenes are caught early.
			if (Collision == null)
				throw new InvalidOperationException("Collision is not assigned on Planet.");
			if (Health == null)
				throw new InvalidOperationException("HealthComponent is not assigned on Planet.");

			AddToGroup("ProjectileImmune");
			ConnectSignals();
		}

		public override void _ExitTree()
		{
			DisconnectSignals();
		}

		// ------------------------------ Signal management ----------------------------------

		private void ConnectSignals()
		{
			Health.Death += OnDeath;
		}

		private void DisconnectSignals()
		{
			Health.Death -= OnDeath;
		}

		// ------------------------------ Signal handlers ----------------------------------

		private void OnDeath()
		{
			SceneController.Instance.ChangeScene(SceneType.MainMenu);
		}
	}
}