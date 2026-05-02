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
		[Export] private Timer _asteroidSpawner;
		[Export] private Player _player;
		[Export] private Planet _planet;
		[Export] private Score _score;

		// Internal state for difficulty tracking.
		private float _currentSpeedMultiplier = 0;
		private int _lastDifficultyTier = 0;

		// Difficulty scaling constants — tweak to balance the game.
		private const int PointsPerDifficultyTier = 1000;
		private const int MaxDifficultyTier = 10;
		private const float BaseSpawnInterval = 2.0f;
		private const float SpawnRateIncreasePerTier = 0.25f;
		private const float SpeedIncreasePerTier = 1f;
		private const float SpeedIncreaseScale = 10f;

		// ------------------------------------- Godot overrides ------------------------------------

		// Registered in _EnterTree so no asteroid spawned before _Ready is missed.
		public override void _EnterTree() => GetTree().NodeAdded += OnNodeAdded;

		public override void _Ready() => ConnectSignals();

		public override void _ExitTree()
		{
			GetTree().NodeAdded -= OnNodeAdded;
			DisconnectSignals();
		}

		// ------------------------------------ Signal management -----------------------------------

		private void ConnectSignals()
		{
			_score.ScoreChanged += OnScoreChanged;
			_planet.PlanetDestroyed += Gameover;
			_player.PlayerDied += Gameover;
		}

		private void DisconnectSignals()
		{
			_score.ScoreChanged -= OnScoreChanged;
			_planet.PlanetDestroyed -= Gameover;
			_player.PlayerDied -= Gameover;
		}

		private void OnNodeAdded(Node node)
		{
			if (node is not Asteroid asteroid) return;

			asteroid.AddSpeed(_currentSpeedMultiplier * SpeedIncreaseScale);
			asteroid.AsteroidDestroyed += OnAsteroidDestroyed;
			asteroid.TreeExiting += () => asteroid.AsteroidDestroyed -= OnAsteroidDestroyed;
		}

		private void OnAsteroidDestroyed(int value) => _score.IncreaseScore(value);

		private void OnScoreChanged(int newScore) => IncreaseDifficulty(newScore);

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
			_asteroidSpawner.WaitTime = BaseSpawnInterval / spawnRateMultiplier;
		}

		// --------------------------------------- Game flow ----------------------------------------

		private static void Gameover() => SceneController.Instance.ChangeScene(SceneType.Gameover);
	}
}