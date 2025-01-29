using Godot;

public partial class game : Node2D
{
	public const int Minimum = 50;
	public const int Maximum = 950;
	private static int cellSize;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cellSize = (Maximum - Minimum) / 8;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public static Vector2I ConvertToCell(Vector2 pos)
	{
		pos = pos.Clamp(new Vector2(Minimum, Minimum), new Vector2(Maximum - 1, Maximum - 1));

		static int mapValue(float a)
		{
			a -= Minimum;
			a /= Maximum - Minimum;
			a *= 8;
			return (int)a;
		}

		return new Vector2I(mapValue(pos.X), mapValue(pos.Y));
	}

	public static Vector2 ConvertFromCell(Vector2I cell)
	{
		return new Vector2(
	 		2 + cellSize / 2 + Minimum + (cell.X * cellSize),
	 		2 + cellSize / 2 + Minimum + (cell.Y * cellSize)
		);
	}
}
