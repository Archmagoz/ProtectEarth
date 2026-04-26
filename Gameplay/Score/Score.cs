using Godot;

namespace ProtectEarth.Gameplay.Score
{
	[GlobalClass]
	public partial class Score : Node2D
	{
		// Signals must be the only the only way to notify score changes to other parts of the game, such as UI or game manager.
		[Signal] public delegate void ScoreChangedEventHandler(int newScore);
		[Signal] public delegate void ScoreResetEventHandler();

		// Node reference for the score display label, assigned via editor or auto-resolved in _Ready.
		[Export] public RichTextLabel ScoreLabel { get; private set; }

		// Internal state for the current score.
		public int CurrentScore { get; private set; } = 0;

		public override void _Ready()
		{
			// Fallback to find nodes if not set via editor.
			ScoreLabel ??= GetNodeOrNull<RichTextLabel>("ScoreLabel");
		}

		// Helper method to update the score label text and emit the ScoreChanged signal.
		private void UpdateLabel() => ScoreLabel.Text = CurrentScore.ToString();

		// Public API to modify the score.
		public void IncreaseScore(int value)
		{
			CurrentScore += value;
			UpdateLabel();
			EmitSignal(SignalName.ScoreChanged, CurrentScore);
		}

		public void ResetScore()
		{
			CurrentScore = 0;
			UpdateLabel();
			EmitSignal(SignalName.ScoreReset);
		}
	}
}
