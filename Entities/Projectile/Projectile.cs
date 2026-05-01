using ProtectEarth.Core.Interfaces;
using ProtectEarth.Components;

using Godot;
using System;

namespace ProtectEarth.Entities
{
	[GlobalClass]
	public partial class Projectile : Area2D
	{
		// Node references (assigned via editor).
		[ExportGroup("Components")]
		[Export] public AnimatedSprite2D AnimatedSprite { get; private set; }
		[Export] public CollisionShape2D Collision { get; private set; }
		[Export] public SpeedComponent Speed { get; private set; }
		[Export] public Timer LifetimeTimer { get; private set; }

		// Gameplay parameters (assigned via editor).
		[ExportGroup("Gameplay")]
		[Export] public AudioStream SoundEffectStream { get; private set; }
		[Export] public int Damage { get; set; } = 100;

		// Set by the Player when instantiating this projectile.
		public Vector2 Direction { get; set; }

		// ------------------------------ Godot overrides ----------------------------------

		public override void _Ready()
		{
			// Hard validation — these components are required for the Projectile to function.
			// Throws in all build configurations, ensuring misconfigured scenes are caught early.
			if (AnimatedSprite == null)
				throw new InvalidOperationException("AnimatedSprite is not assigned on Projectile.");
			if (Collision == null)
				throw new InvalidOperationException("Collision is not assigned on Projectile.");
			if (Speed == null)
				throw new InvalidOperationException("SpeedComponent is not assigned on Projectile.");
			if (LifetimeTimer == null)
				throw new InvalidOperationException("LifetimeTimer is not assigned on Projectile.");

			// SoundEffectStream is optional — PlaySoundIndependent handles the null case.
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
			AreaEntered += OnAreaEntered;
			LifetimeTimer.Timeout += OnLifetimeTimeout;
		}

		private void DisconnectSignals()
		{
			AreaEntered -= OnAreaEntered;
			LifetimeTimer.Timeout -= OnLifetimeTimeout;
		}

		// ------------------------------ Signal handlers ----------------------------------

		private void OnAreaEntered(Area2D entity)
		{
			if (entity.IsInGroup("ProjectileImmune")) return;

			if (entity is IDamageable damageable)
			{
				damageable.ApplyDamage(Damage);
				QueueFree();
			}
		}

		private void OnLifetimeTimeout() => QueueFree();

		// ------------------------------ Sound ----------------------------------

		// Spawns a fire-and-forget AudioStreamPlayer2D at root level so it survives projectile deletion.
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