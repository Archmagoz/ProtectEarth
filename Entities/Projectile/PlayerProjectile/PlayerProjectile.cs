using ProtectEarth.Core.Interfaces;
using ProtectEarth.Components;

using Godot;

namespace ProtectEarth.Entities.Projectile
{
	[GlobalClass]
	public partial class PlayerProjectile : Area2D
	{
		// Node references (assigned via editor or auto-resolved in _Ready).
		[Export] public AnimatedSprite2D AnimatedSprite { get; private set; }
		[Export] public CollisionShape2D Collision { get; private set; }
		[Export] public SpeedComponent Speed { get; private set; }

		// Sound effect stream to play on spawn, auto-freed when finished.
		[Export] public AudioStream SoundEffectStream { get; private set; }

		// Damage value for the projectile, can be set via editor or code.
		[Export] public int Damage { get; set; } = 100;

		// Source reference to avoid self-collision.
		public Node Source { get; set; }

		// Direction vector for movement, set by the player when shooting.
		public Vector2 Direction { get; set; }

		public override void _Ready()
		{
			// Fallback to find nodes if not set via editor.
			AnimatedSprite ??= GetNodeOrNull<AnimatedSprite2D>("Sprite");
			Collision ??= GetNodeOrNull<CollisionShape2D>("Collision");
			Speed ??= GetNodeOrNull<SpeedComponent>("SpeedComponent");

			// Play sound as independent node in root so it survives projectile deletion.
			PlaySoundIndependent();

			// Connect signals.
			BodyEntered += OnBodyEntered;
		}

		public override void _PhysicsProcess(double delta)
		{
			Position += Direction * Speed.CurrentSpeed * (float)delta;
		}

		// Spawns a fire-and-forget AudioStreamPlayer2D at the root level.
		private void PlaySoundIndependent()
		{
			if (SoundEffectStream == null) return;

			var player = new AudioStreamPlayer2D()
			{
				Stream = SoundEffectStream,
				PitchScale = 2.0f,
				VolumeDb = -5.0f,
				GlobalPosition = GlobalPosition,
			};

			GetTree().Root.AddChild(player);

			player.Play();

			// Auto-free when the sound finishes.
			player.Finished += player.QueueFree;
		}

		// Checks for collisions with damageable entities, applies damage and frees the projectile.
		private void OnBodyEntered(Node body)
		{
			if (body == Source) return; // Ignore collisions with the source (player)
			if (body is Planet) return; // Ignore planet collision.

			if (body is IDamageable damageable)
			{
				damageable.ApplyDamage(Damage);
				QueueFree();
			}
		}

		// Free the projectile when Lifetime Timer ends, timer set by godot editor.
		private void OnLifetimeTimeout() => QueueFree();
	}
}
