using Godot;

public partial class game : Node2D
{
	public const int Minimum = 50;
	public const int Maximum = 950;
	private CellHighlight highlighter;
	private Vector2I previousMouseCell;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		highlighter = (CellHighlight)FindChild("CellHighlight", false);
		previousMouseCell = new(-1, -1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2I currentMouseCell = ConvertToCell(GetViewport().GetMousePosition());
		if (currentMouseCell.Equals(previousMouseCell))
		{
			return;
		}
		highlighter.DeselectCell(previousMouseCell);
		highlighter.SelectCell(currentMouseCell);
		previousMouseCell = currentMouseCell;
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
			// Square highlight is scaled by a factor of 1.75
			float scalingFactor = 1f / 1.75f;
			// I have NO IDEA why I need this number or why it is 12
			int magicNumber = 12;

			float cellSize = (Maximum - Minimum) / 8;
			float a = x * cellSize * scalingFactor;
			return a + Minimum + magicNumber;
		}

		return new Vector2(unmapValue(cell.X), unmapValue(cell.Y));
	}
}
