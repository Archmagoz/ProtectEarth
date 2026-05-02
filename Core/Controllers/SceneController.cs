using System.Collections.Generic;

using Godot;

namespace ProtectEarth.Core.Controllers
{
	public enum SceneType
	{
		DebugLevel,
		MainMenu,
		Gameover,
	}

	public partial class SceneController : Node
	{
		public static SceneController Instance { get; private set; }

		// Preloaded scenes — instantiated immediately, no runtime disk reads.
		private readonly Dictionary<SceneType, PackedScene> _preloaded = new()
		{
			{ SceneType.MainMenu, GD.Load<PackedScene>("res://UI/MainMenu/MainMenu.tscn") },
		};

		// Lazy-loaded scenes — resolved on first request, then promoted to _preloaded.
		private readonly Dictionary<SceneType, string> _paths = new()
		{
			{ SceneType.DebugLevel, "res://Levels/DebugLevel/DebugLevel.tscn" },
			{ SceneType.Gameover, "res://UI/Gameover/Gameover.tscn" },
		};

		// ------------------------------------- Godot overrides ------------------------------------

		public override void _Ready() => Instance = this;

		// --------------------------------------- Public API ---------------------------------------

		// Swaps the current scene for the requested type; no-ops silently if type is unregistered.
		public void ChangeScene(SceneType type)
		{
			if (!TryGetScene(type, out var scene)) return;

			GetTree().CurrentScene?.QueueFree();

			var newScene = scene.Instantiate();
			GetTree().Root.AddChild(newScene);
			GetTree().CurrentScene = newScene;
		}

		// ---------------------------------------- Helpers ----------------------------------------

		// Resolves a SceneType to a PackedScene, lazy-loading and caching if necessary.
		private bool TryGetScene(SceneType type, out PackedScene scene)
		{
			if (_preloaded.TryGetValue(type, out scene))
				return true;

			if (_paths.TryGetValue(type, out var path))
			{
				scene = GD.Load<PackedScene>(path);
				_preloaded[type] = scene; // promote to preloaded — skips disk on subsequent calls.
				return true;
			}

			scene = null;
			return false;
		}
	}
}