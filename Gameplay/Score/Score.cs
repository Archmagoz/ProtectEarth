using Godot;

namespace ProtectEarth.Gameplay
{
	[GlobalClass]
	public partial class Score : Control
	{
		// Signal Handlers.
		[Signal] public delegate void ScoreChangedEventHandler(int newScore);
		[Signal] public delegate void ScoreResetEventHandler();

		// Node reference (assigned via editor).
		[Export] private RichTextLabel _scoreLabel;

		// Runtime state — current accumulated score value.
		public int CurrentScore { get; private set; } = 0;

		// ---------------------------------------- Public API --------------------------------------

		public void IncreaseScore(int value)
		{
			// Accumulates score and notifies listeners about the updated value.
			CurrentScore += value;
			UpdateLabel();
			EmitSignal(SignalName.ScoreChanged, CurrentScore);
		}

		public void ResetScore()
		{
			// Resets score state and informs dependent systems.
			CurrentScore = 0;
			UpdateLabel();
			EmitSignal(SignalName.ScoreReset);
		}

		// ----------------------------------------- Helpers ----------------------------------------

		private void UpdateLabel()
		{
			// Synchronizes UI representation with internal score state.
			_scoreLabel.Text = CurrentScore.ToString();
		}
	}
}