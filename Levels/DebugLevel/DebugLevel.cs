using ProtectEarth.Core.Utils;
using ProtectEarth.Entities;

using Godot;
using System;

namespace ProtectEarth.Levels
{
	public partial class DebugLevel : Node2D
	{
		// Scene reference (assigned via editor).
		[Export] private PackedScene _asteroidScene;

		// ------------------------------ Godot overrides ----------------------------------

		public override void _Ready()
		{
			// Hard validation — _asteroidScene is required for the DebugLevel to function.
			// Throws in all build configurations, ensuring misconfigured scenes are caught early.
			if (_asteroidScene == null)
				throw new InvalidOperationException("AsteroidScene is not assigned on DebugLevel.");
		}

		// ------------------------------ Signal handlers ----------------------------------

		public void OnAsteroidSpawnerTimeout()
		{
			var asteroid = _asteroidScene.Instantiate<Asteroid>();
			asteroid.GlobalPosition = GetSpawnPosition();
			AddChild(asteroid);
		}

		// ------------------------------ Helpers ----------------------------------

		private Vector2 GetSpawnPosition(float margin = 50f)
		{
			var camera = GetViewport().GetCamera2D();

			if (camera == null) return Vector2.Zero;

			var screenSize = GetViewport().GetVisibleRect().Size;
			var zoom = camera.Zoom;
			var halfSize = screenSize * 0.5f / zoom;
			var center = camera.GlobalPosition;
			var left = center.X - halfSize.X;
			var right = center.X + halfSize.X;
			var top = center.Y - halfSize.Y;
			var bottom = center.Y + halfSize.Y;

			return RNG.Range(0, 4) switch
			{
				0 => new Vector2(RNG.Range(left, right), top - margin),    // Upper
				1 => new Vector2(RNG.Range(left, right), bottom + margin), // Bottom
				2 => new Vector2(left - margin, RNG.Range(top, bottom)),   // Left
				3 => new Vector2(right + margin, RNG.Range(top, bottom)),  // Right
				_ => new Vector2(left - margin, top - margin)
			};
		}
	}
}