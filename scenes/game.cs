using System;
using Godot;
using static Board;

public partial class Game : Node2D
{
	public const int Minimum = 50;
	public const int Maximum = 950;
	public const int BoardSize = 8;
	public const float cellSize = (Maximum - Minimum) / BoardSize;

	private CellHighlight highlighter;
	private Board board;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		highlighter = (CellHighlight)FindChild("CellHighlight", false);
		board = (Board)FindChild("Board", false);

		board.ResetBoard();
		DrawBoard();
	}

	public void DrawBoard()
	{
		foreach (var child in GetChildren())
		{
			if (child.Equals(board) || child.Equals(highlighter))
			{
				continue;
			}
			child.QueueFree();
		}
		for (int y = 0; y < BoardSize; y++)
		{
			for (int x = 0; x < BoardSize; x++)
			{
				Vector2I cell = new(x, y);
				Piece? cellContent = board.GetPiece(cell);
				if (cellContent == null)
				{
					continue;
				}
				Piece piece = (Piece)cellContent;

				Sprite2D sprite = new()
				{
					Texture = piece.GetTexture(),
					Centered = true,
					Scale = new(4f, 4f),
					Position = ConvertFromCell(cell),
					ZIndex = 100,
					Offset = new(0, -3)
				};
				AddChild(sprite);
			}
		}
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
			a *= BoardSize;
			return (int)a;
		}

		return new Vector2I(mapValue(pos.X), mapValue(pos.Y));
	}

	public static Vector2 ConvertFromCell(Vector2I cell)
	{
		Func<int, float> unmap = x => 2 + Minimum + x * cellSize + cellSize / 2;

		return new Vector2(unmap(cell.X), unmap(cell.Y));
	}
}
