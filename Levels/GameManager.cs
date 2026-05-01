using ProtectEarth.Core.Controllers;
using ProtectEarth.Gameplay;
using ProtectEarth.Entities;

using Godot;

namespace ProtectEarth.Levels
{
	public partial class GameManager : Node
	{
		// Node references (assigned via editor).
		[ExportGroup("Components")]
		[Export] public Timer AsteroidSpawner { get; private set; }
		[Export] public Score Score { get; private set; }

		[ExportGroup("Gameplay")]
		[Export] public Planet Planet { get; private set; }

		// Difficulty scaling constants — tweak to balance the game.
		private const float BaseSpawnInterval = 2.0f;
		private const int PointsPerDifficultyTier = 1000;
		private const float SpawnRateIncreasePerTier = 0.15f;
		private const float SpeedIncreasePerTier = 1f;
		private const int MaxDifficultyTier = 10;
		private const float SpeedIncreaseScale = 10f;

		// Internal state for difficulty tracking.
		private int _lastDifficultyTier = 0;
		private float _currentSpeedMultiplier = 0;

		// ------------------------------------- Godot overrides ------------------------------------

		// Registered in _EnterTree so no asteroid spawned before _Ready is missed.
		public override void _EnterTree() => GetTree().NodeAdded += OnNodeAdded;

		public override void _Ready()
		{
			ConnectSignals();
		}

		public override void _ExitTree()
		{
			GetTree().NodeAdded -= OnNodeAdded;
			DisconnectSignals();
		}

		// ------------------------------------ Signal management -----------------------------------

		private void ConnectSignals()
		{
			Score.ScoreChanged += OnScoreChanged;
			Planet.PlanetDestroyed += OnPlanetDestroyed;
		}

		private void DisconnectSignals()
		{
			Score.ScoreChanged -= OnScoreChanged;
			Planet.PlanetDestroyed -= OnPlanetDestroyed;
		}

		// ------------------------------------ Signal handlers -------------------------------------

		private void OnNodeAdded(Node node)
		{
			if (node is not Asteroid asteroid) return;

			asteroid.AddSpeed(_currentSpeedMultiplier * SpeedIncreaseScale);
			asteroid.AsteroidDestroyed += OnAsteroidDestroyed;
			asteroid.TreeExiting += () => asteroid.AsteroidDestroyed -= OnAsteroidDestroyed;
		}

		private void OnAsteroidDestroyed(int value) => Score.IncreaseScore(value);

		private void OnScoreChanged(int newScore) => IncreaseDifficulty(newScore);

		private void OnPlanetDestroyed() => Gameover();

		// ----------------------------------- Difficulty scaling -----------------------------------

		private void IncreaseDifficulty(int score)
		{
			var currentTier = Mathf.Min(score / PointsPerDifficultyTier, MaxDifficultyTier);

			if (currentTier <= _lastDifficultyTier) return;

			_lastDifficultyTier = currentTier;

			ApplyDifficulty(
				spawnRateMultiplier: 1f + SpawnRateIncreasePerTier * currentTier,
				speedMultiplier: 1f + SpeedIncreasePerTier * currentTier
			);
		}

		private void ApplyDifficulty(float spawnRateMultiplier, float speedMultiplier)
		{
			_currentSpeedMultiplier = speedMultiplier;
			AsteroidSpawner.WaitTime = BaseSpawnInterval / spawnRateMultiplier;
		}

		// --------------------------------------- Game flow ----------------------------------------

		private static void Gameover() => SceneController.Instance.ChangeScene(SceneType.Gameover);
	}
}