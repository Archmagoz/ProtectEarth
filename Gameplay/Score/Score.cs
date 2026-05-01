using ProtectEarth.Core.Utils;

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
		[Export] public RichTextLabel ScoreLabel { get; private set; }

		// Runtime state — current accumulated score value.
		public int CurrentScore { get; private set; } = 0;

		// --------------------------------------- Validation ---------------------------------------

		private bool ValidateNodes()
		{
			bool valid = true;
			valid &= NodeValidator.Require(ScoreLabel, nameof(ScoreLabel), nameof(Score));
			return valid;
		}

		// ------------------------------------- Godot overrides ------------------------------------

		public override void _Ready()
		{
			if (!ValidateNodes())
			{
				GetTree().Quit();
				return;
			}
		}

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
			ScoreLabel.Text = CurrentScore.ToString();
		}
	}
}