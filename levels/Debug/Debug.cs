using Godot;
using ProtectEarth.Entities;
using ProtectEarth.Utils;

namespace ProtectEarth.Levels
{
	public partial class Debug : Node2D
	{
		[Export]
		private PackedScene _asteroidScene;

		public override void _Ready()
		{
			_asteroidScene ??=
				GD.Load<PackedScene>("res://Entities/Asteroid/Asteroid.tscn");
		}

		private Vector2 GetSpawnPosition(float margin = 50f)
		{
			var camera = GetViewport().GetCamera2D();
			if (camera == null)
				return Vector2.Zero;

			var screenSize = GetViewport().GetVisibleRect().Size;

			var zoom = camera.Zoom;

			var halfSize = screenSize * 0.5f / zoom;

			var center = camera.GlobalPosition;

			var left = center.X - halfSize.X;
			var right = center.X + halfSize.X;
			var top = center.Y - halfSize.Y;
			var bottom = center.Y + halfSize.Y;

			int side = Rng.Range(0, 4);

			return side switch
			{
				// Upper
				0 => new Vector2(Rng.Range(left, right), top - margin),

				// Bottom
				1 => new Vector2(Rng.Range(left, right), bottom + margin),

				// Left
				2 => new Vector2(left - margin, Rng.Range(top, bottom)),

				// Right
				3 => new Vector2(right + margin, Rng.Range(top, bottom)),

				_ => new Vector2(left - margin, top - margin)
			};
		}

		public void OnAsteroidSpawnerTimeout()
		{
			var asteroid = _asteroidScene.Instantiate<Asteroid>();
			asteroid.GlobalPosition = GetSpawnPosition(50f);
			AddChild(asteroid);
		}
	}
}