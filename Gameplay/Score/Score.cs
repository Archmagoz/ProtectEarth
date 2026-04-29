using Godot;

namespace ProtectEarth.Gameplay
{
	[GlobalClass]
	public partial class Score : Control
	{
		[Signal] public delegate void ScoreChangedEventHandler(int newScore);
		[Signal] public delegate void ScoreResetEventHandler();

		[Export] public RichTextLabel ScoreLabel { get; private set; }

		public int CurrentScore { get; private set; } = 0;

		// ------------------------------ Godot overrides ----------------------------------

		public override void _Ready() =>
			ScoreLabel ??= GetNodeOrNull<RichTextLabel>("ScoreLabel");

		// ------------------------------ Public API ----------------------------------

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

		// ------------------------------ Helpers ----------------------------------

		private void UpdateLabel() => ScoreLabel.Text = CurrentScore.ToString();
	}
}