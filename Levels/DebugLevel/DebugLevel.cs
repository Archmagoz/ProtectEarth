using ProtectEarth.Core.Utils;
using ProtectEarth.Entities;

using Godot;

namespace ProtectEarth.Levels
{
	public partial class DebugLevel : Node2D
	{
		[Export] private PackedScene _asteroidScene;

		// ------------------------------ Godot overrides ----------------------------------

		public override void _Ready() =>
			_asteroidScene ??= GD.Load<PackedScene>("res://Entities/Asteroid/Asteroid.tscn");

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