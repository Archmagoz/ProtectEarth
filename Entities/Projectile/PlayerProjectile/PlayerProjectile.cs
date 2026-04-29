using ProtectEarth.Core.Interfaces;
using ProtectEarth.Components;

using Godot;

namespace ProtectEarth.Entities.Projectile
{
	[GlobalClass]
	public partial class PlayerProjectile : Area2D
	{
		// Node references (assigned via editor or auto-resolved in _Ready).
		[ExportGroup("Components")]
		[Export] public AnimatedSprite2D AnimatedSprite { get; private set; }
		[Export] public CollisionShape2D Collision { get; private set; }
		[Export] public SpeedComponent Speed { get; private set; }
		[Export] public Timer LifetimeTimer { get; private set; }

		[ExportGroup("Gameplay")]
		[Export] public AudioStream SoundEffectStream { get; private set; }
		[Export] public int Damage { get; set; } = 100;

		// Set by the player when shooting.
		public Node Source { get; set; }
		public Vector2 Direction { get; set; }

		// ------------------------------ Godot overrides ----------------------------------

		public override void _Ready()
		{
			AnimatedSprite ??= GetNodeOrNull<AnimatedSprite2D>("Sprite");
			Collision ??= GetNodeOrNull<CollisionShape2D>("Collision");
			Speed ??= GetNodeOrNull<SpeedComponent>("SpeedComponent");
			LifetimeTimer ??= GetNodeOrNull<Timer>("Lifetime");

			// Play sound as independent node in root so it survives projectile deletion.
			PlaySoundIndependent();

			ConnectSignals();
		}

		public override void _PhysicsProcess(double delta) =>
			Position += Direction * Speed.CurrentSpeed * (float)delta;

		public override void _ExitTree()
		{
			DisconnectSignals();
		}

		// ------------------------------ Signal management ----------------------------------

		private void ConnectSignals()
		{
			BodyEntered += OnBodyEntered;
			LifetimeTimer.Timeout += OnLifetimeTimeout;
		}

		private void DisconnectSignals()
		{
			BodyEntered -= OnBodyEntered;
			LifetimeTimer.Timeout -= OnLifetimeTimeout;
		}

		// ------------------------------ Signal handlers ----------------------------------

		private void OnBodyEntered(Node body)
		{
			if (body == Source) return;
			if (body is Planet) return;

			if (body is IDamageable damageable)
			{
				damageable.ApplyDamage(Damage);
				QueueFree();
			}
		}

		private void OnLifetimeTimeout() => QueueFree();

		// ------------------------------ Sound ----------------------------------

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
			player.Finished += player.QueueFree;
		}
	}
}