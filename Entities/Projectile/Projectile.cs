using ProtectEarth.Core.Interfaces;
using ProtectEarth.Components;

using Godot;

namespace ProtectEarth.Entities
{
	[GlobalClass]
	public partial class Projectile : Area2D
	{
		// Node references (assigned via editor).
		[ExportGroup("Components")]
		[Export] private AnimatedSprite2D _animatedSprite;
		[Export] private CollisionShape2D _collision;
		[Export] private SpeedComponent _speed;

		// Gameplay parameters (assigned via editor).
		[ExportGroup("Gameplay")]
		[Export] private AudioStream _soundEffectStream;
		[Export] private int _damage = 100;

		// Runtime state — injected by the spawner (Player).
		public Vector2 Direction;

		// Runtime state (internal)
		private float _lifetime = 3f;

		// ------------------------------------- Godot overrides ------------------------------------

		public override void _Ready()
		{
			PlaySoundIndependent();
			ConnectSignals();
		}

		public override void _PhysicsProcess(double delta)
		{
			_lifetime -= (float)delta;
			if (_lifetime <= 0f) QueueFree();

			Translate(Direction * _speed.CurrentSpeed * (float)delta);
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

		private void OnAreaEntered(Area2D entity)
		{
			// Ignore entities explicitly marked as immune to projectile interactions.
			if (entity.IsInGroup("ProjectileImmune")) return;

			// Apply damage only to entities that implement the damage contract.
			if (entity is IDamageable damageable)
			{
				damageable.ApplyDamage(_damage);
				QueueFree();
			}
		}

		private void OnLifetimeTimeout() => QueueFree();

		// ---------------------------------------- Audio -------------------------------------------

		// Spawns a detached AudioStreamPlayer2D at root level so playback persists after projectile destruction.
		private void PlaySoundIndependent()
		{
			if (_soundEffectStream == null) return;

			var player = new AudioStreamPlayer2D()
			{
				Stream = _soundEffectStream,
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