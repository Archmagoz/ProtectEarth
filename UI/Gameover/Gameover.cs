using ProtectEarth.Core.Controllers;

using Godot;

namespace ProtectEarth.UI
{
	public partial class Gameover : Node2D
	{
		// Node references (assigned via editor).
		[ExportGroup("Buttons")]
		[Export] private TextureButton _retryButton;
		[Export] private TextureButton _quitButton;

		public override void _Ready()
		{
			_retryButton.Pressed += OnRetryPressed;
			_quitButton.Pressed += OnQuitPressed;
		}

		private void OnRetryPressed() => SceneController.Instance.ChangeScene(SceneType.DebugLevel);
		private void OnQuitPressed() => GetTree().Quit();
	}
}