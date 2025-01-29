using Godot;

public partial class game : Node2D
{
	public const int Minimum = 50;
	public const int Maximum = 950;
	private CellHighlight highlighter;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		highlighter = (CellHighlight)FindChild("CellHighlight", false);
		for (int i = 0; i < 8; i++)
		{
			highlighter.SelectCell(new(i, i));
		}
		highlighter.DeselectAllCells();
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
		static float unmapValue(int x)
		{
			// I have NO IDEA why a) I need these numbers b) why they are the values that they are 
			float magicNumberOne = 4 / 7;
			int magicNumberTwo = 12;

			float cellSize = (Maximum - Minimum) / 8;
			float a = x * cellSize * magicNumberOne;
			return a + Minimum + magicNumberTwo;
		}

		return new Vector2(unmapValue(cell.X), unmapValue(cell.Y));
	}
}
