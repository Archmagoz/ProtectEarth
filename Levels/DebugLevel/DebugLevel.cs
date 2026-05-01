using ProtectEarth.Core.Utils;
using ProtectEarth.Entities;

using Godot;

namespace ProtectEarth.Levels
{
	public partial class DebugLevel : Node2D
	{
		// Scene reference (assigned via editor).
		[Export] private PackedScene _asteroidScene;

		// --------------------------------------- Validation ---------------------------------------

		private bool ValidateNodes()
		{
			bool valid = true;
			valid &= NodeValidator.Require(_asteroidScene, nameof(_asteroidScene), nameof(DebugLevel));
			return valid;
		}

		// ------------------------------------- Godot overrides ------------------------------------

		public override void _Ready()
		{
			if (!ValidateNodes())
			{
				GetTree().Quit();
				return;
			}
		}

		// ------------------------------------ Signal handlers -------------------------------------

		public void OnAsteroidSpawnerTimeout()
		{
			var asteroid = _asteroidScene.Instantiate<Asteroid>();
			asteroid.GlobalPosition = GetSpawnPosition();
			AddChild(asteroid);
		}

		// ---------------------------------------- Helpers ----------------------------------------

		// Picks a spawn position just outside the visible viewport on a random edge.
		private Vector2 GetSpawnPosition(float margin = 50f)
		{
			var camera = GetViewport().GetCamera2D();
			if (camera == null) return Vector2.Zero;

			var screenSize = GetViewport().GetVisibleRect().Size;
			var halfSize = screenSize * 0.5f / camera.Zoom;
			var center = camera.GlobalPosition;

			var left = center.X - halfSize.X;
			var right = center.X + halfSize.X;
			var top = center.Y - halfSize.Y;
			var bottom = center.Y + halfSize.Y;

			return RNG.Range(0, 4) switch
			{
				0 => new Vector2(RNG.Range(left, right), top - margin), // top
				1 => new Vector2(RNG.Range(left, right), bottom + margin), // bottom
				2 => new Vector2(left - margin, RNG.Range(top, bottom)),  // left
				3 => new Vector2(right + margin, RNG.Range(top, bottom)),  // right
				_ => new Vector2(left - margin, top - margin)
			};
		}
	}
}