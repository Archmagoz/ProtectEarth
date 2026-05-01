using ProtectEarth.Core.Interfaces;
using ProtectEarth.Core.Utils;
using ProtectEarth.Components;

using Godot;

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

		// Gameplay parameters (assigned via editor).
		[ExportGroup("Gameplay")]
		[Export] public AudioStream SoundEffectStream { get; private set; }
		[Export] public int Damage { get; set; } = 100;

		// Runtime state — injected by the spawner (Player).
		public Vector2 Direction { get; set; }

		// Runtime state (internal)
		private float _lifetime = 3f;

		// ------------------------------------- Godot overrides ------------------------------------

		public override void _Ready()
		{
			this.ValidateExports();

			PlaySoundIndependent();
			ConnectSignals();
		}

		public override void _PhysicsProcess(double delta)
		{
			_lifetime -= (float)delta;
			if (_lifetime <= 0f) QueueFree();

			Translate(Direction * Speed.CurrentSpeed * (float)delta);
		}

		public override void _ExitTree() => DisconnectSignals();

		// ------------------------------------ Signal management -----------------------------------

		private void ConnectSignals()
		{
			AreaEntered += OnAreaEntered;
		}

		private void DisconnectSignals()
		{
			AreaEntered -= OnAreaEntered;
		}

		// ------------------------------------ Signal handlers -------------------------------------

		private void OnAreaEntered(Area2D entity)
		{
			// Ignore entities explicitly marked as immune to projectile interactions.
			if (entity.IsInGroup("ProjectileImmune")) return;

			// Apply damage only to entities that implement the damage contract.
			if (entity is IDamageable damageable)
			{
				damageable.ApplyDamage(Damage);
				QueueFree();
			}
		}

		private void OnLifetimeTimeout() => QueueFree();

		// ---------------------------------------- Audio -------------------------------------------

		// Spawns a detached AudioStreamPlayer2D at root level so playback persists after projectile destruction.
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