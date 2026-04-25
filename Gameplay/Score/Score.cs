using Godot;

namespace ProtectEarth.Gameplay.Score
{
	public partial class Score : Node2D
	{
		// Signals must be the only accoplament in this class, as it's purely for score management.
		[Signal] public delegate void ScoreChangedEventHandler(int newScore);
		[Signal] public delegate void ScoreResetEventHandler();

		// Internal state for the current score.
		public int CurrentScore { get; private set; } = 0;

		// Public API to modify the score.
		public void IncreaseScore(int value)
		{
			if (value < 0) return;

			CurrentScore += value;
			EmitSignal(SignalName.ScoreChanged, CurrentScore);
		}

		public void ResetScore()
		{
			CurrentScore = 0;
			EmitSignal(SignalName.ScoreReset);
		}
	}
}