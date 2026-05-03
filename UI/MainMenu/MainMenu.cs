using ProtectEarth.Core.Controllers;

using Godot;

namespace ProtectEarth.UI
{
	public partial class MainMenu : Node2D
	{
		// Node references (assigned via editor).
		[ExportGroup("Buttons")]
		[Export] private TextureButton _startButton;
		[Export] private TextureButton _quitButton;

		public override void _Ready()
		{
			_startButton.Pressed += OnStartPressed;
			_quitButton.Pressed += OnQuitPressed;
		}

		private void OnStartPressed() => SceneController.Instance.ChangeScene(SceneType.DebugLevel);
		private void OnQuitPressed() => GetTree().Quit();
	}
}
