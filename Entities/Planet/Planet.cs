using ProtectEarth.Core.Interfaces;
using ProtectEarth.Components;

using Godot;

namespace ProtectEarth.Entities
{
	public partial class Planet : Node2D, IDamageable
	{
		// Node references (assigned via editor or auto-resolved in _Ready).
		[Export] public HealthComponent Health { get; private set; }
		[Export] public CollisionShape2D Collision { get; private set; }

		// IDamageable delegate to HealthComponent.
		public void ApplyDamage(int damage) => Health.ApplyDamage(damage);

		public override void _Ready()
		{
			// Fallback to find nodes if not set via editor.
			Health ??= GetNodeOrNull<HealthComponent>("HealthComponent");
			Collision ??= GetNodeOrNull<CollisionShape2D>("Collision");
		}
	}
}
