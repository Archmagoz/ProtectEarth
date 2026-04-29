using System.Collections.Generic;
using Godot;

namespace ProtectEarth.Core.Controllers
{
	public enum SceneType
	{
		DebugLevel,
		MainMenu,
	}

	public partial class SceneController : Node
	{
		public static SceneController Instance { get; private set; }

		// Preloaded scenes (fast access, no runtime load).
		private readonly Dictionary<SceneType, PackedScene> _preloaded = new()
		{
			{ SceneType.MainMenu, GD.Load<PackedScene>("res://UI/MainMenu/MainMenu.tscn") },
		};

		// Lazy-loaded scenes (loaded only when needed).
		private readonly Dictionary<SceneType, string> _paths = new()
		{
			{ SceneType.DebugLevel, "res://Levels/DebugLevel/DebugLevel.tscn" },
		};

		private Node _currentScene;

		// ------------------------------ Godot overrides ----------------------------------

		public override void _Ready() => Instance = this;

		// ------------------------------ Public API ----------------------------------

		public void ChangeScene(SceneType type)
		{
			if (!TryGetScene(type, out var scene))
				return;

			_currentScene = GetTree().CurrentScene;
			_currentScene?.QueueFree();

			var newScene = scene.Instantiate();
			GetTree().Root.AddChild(newScene);
			GetTree().CurrentScene = newScene;
		}

		// ------------------------------ Helpers ----------------------------------

		private bool TryGetScene(SceneType type, out PackedScene scene)
		{
			if (_preloaded.TryGetValue(type, out scene))
				return true;

			if (_paths.TryGetValue(type, out var path))
			{
				scene = GD.Load<PackedScene>(path);
				return true;
			}

			scene = null;
			return false;
		}
	}
}