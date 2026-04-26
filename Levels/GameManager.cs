using ProtectEarth.Gameplay;
using ProtectEarth.Entities;

using Godot;

namespace ProtectEarth.Levels
{
	public partial class GameManager : Node
	{
		// Node references (assigned via editor or auto-resolved in _Ready).
		[Export] public Timer AsteroidSpawner { get; private set; }
		[Export] public Score Score { get; private set; }

		// Difficulty scaling constants — tweak to balance the game.
		private const float BaseSpawnInterval = 2.0f;
		private const int PointsPerDifficultyTier = 1000;
		private const float SpawnRateIncreasePerTier = 0.15f;
		private const float SpeedIncreasePerTier = 1f;
		private const int MaxDifficultyTier = 10;

		// Internal state for difficulty tracking.
		private int _lastDifficultyTier = 0;
		private float _currentSpeedMultiplier = 0;

		// Register NodeAdded in _EnterTree so no asteroid is missed before _Ready runs.
		public override void _EnterTree() => GetTree().NodeAdded += OnNodeAdded;

		public override void _Ready()
		{
			// Fallback to find nodes if not set via editor.
			AsteroidSpawner ??= GetNodeOrNull<Timer>("AsteroidSpawner");
			Score ??= GetNodeOrNull<Score>("Score");

			ConnectSignals();
		}

		// Cleaning signal handlers.
		public override void _ExitTree()
		{
			GetTree().NodeAdded -= OnNodeAdded;
			DisconnectSignals();
		}

		// ------------------------------ Signal management ----------------------------------

		private void ConnectSignals()
		{
			Score.ScoreChanged += OnScoreChanged;
		}

		private void DisconnectSignals()
		{
			Score.ScoreChanged -= OnScoreChanged;
		}

		// ------------------------------ Signal handlers ----------------------------------

		// Wire up each new asteroid and apply the current speed multiplier.
		private void OnNodeAdded(Node node)
		{
			if (node is Asteroid asteroid)
			{
				asteroid.AddSpeed(_currentSpeedMultiplier * 10f);
				asteroid.AsteroidDestroyed += OnAsteroidDestroyed;

				asteroid.TreeExited += () => asteroid.AsteroidDestroyed -= OnAsteroidDestroyed;
			}
		}

		private void OnAsteroidDestroyed(int value) => Score.IncreaseScore(value);
		private void OnScoreChanged(int newScore) => IncreaseDifficulty(newScore);

		// ------------------------------ Difficulty scaling ----------------------------------

		// One tier is gained for every PointsPerDifficultyTier points, capped at MaxDifficultyTier.
		private void IncreaseDifficulty(int score)
		{
			var currentTier = Mathf.Min(score / PointsPerDifficultyTier, MaxDifficultyTier);

			if (currentTier <= _lastDifficultyTier) return; // early exit if tier hasn't changed

			_lastDifficultyTier = currentTier;

			var spawnRateMultiplier = 1f + SpawnRateIncreasePerTier * currentTier;
			var speedMultiplier = 1f + SpeedIncreasePerTier * currentTier;

			ApplyDifficulty(spawnRateMultiplier, speedMultiplier);
		}

		private void ApplyDifficulty(float spawnRateMultiplier, float speedMultiplier)
		{
			_currentSpeedMultiplier = speedMultiplier;
			AsteroidSpawner.WaitTime = BaseSpawnInterval / spawnRateMultiplier;
		}
	}
}
