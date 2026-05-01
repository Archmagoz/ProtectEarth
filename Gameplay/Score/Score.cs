using Godot;
using System;

namespace ProtectEarth.Gameplay
{
	[GlobalClass]
	public partial class Score : Control
	{
		// Signal Handlers.
		[Signal] public delegate void ScoreChangedEventHandler(int newScore);
		[Signal] public delegate void ScoreResetEventHandler();

		// Node reference (assigned via editor).
		[Export] public RichTextLabel ScoreLabel { get; private set; }

		public int CurrentScore { get; private set; } = 0;

		// ------------------------------ Godot overrides ----------------------------------

		public override void _Ready()
		{
			// Hard validation — ScoreLabel is required for the Score to function.
			// Throws in all build configurations, ensuring misconfigured scenes are caught early.
			if (ScoreLabel == null)
				throw new InvalidOperationException("ScoreLabel is not assigned on Score.");
		}

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