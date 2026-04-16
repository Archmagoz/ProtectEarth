using Godot;

public partial class Debug : Node2D
{
	private Node2D _planet;
	private float _time = 0f;

	public override void _Ready()
	{
		_planet = GetNode<Node2D>("Planet");
	}

	public override void _Process(double delta)
	{
		_time += (float)delta;

		float speed = 2f;
		float distance = 100;

		float x = Mathf.Sin(_time * speed) * distance;

		_planet.Position = new Vector2(x, _planet.Position.Y);
	}

	public override void _PhysicsProcess(double delta)
	{
	}
}
