using ProtectEarth.Core.Interfaces;
using ProtectEarth.Components;

using Godot;

namespace ProtectEarth.Entities
{
	public partial class Planet : Node2D, IDamageable
	{
		[ExportGroup("Components")]
		[Export] public HealthComponent Health { get; private set; }
		[Export] public CollisionShape2D Collision { get; private set; }

		public void ApplyDamage(int damage) => Health.ApplyDamage(damage);

		// ------------------------------ Godot overrides ----------------------------------

		public override void _Ready()
		{
			Health ??= GetNodeOrNull<HealthComponent>("HealthComponent");
			Collision ??= GetNodeOrNull<CollisionShape2D>("Collision");

			AddToGroup("ImuneToProjectile");
		}
	}
}