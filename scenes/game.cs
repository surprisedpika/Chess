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

	public static Vector2I ConvertToCell(Vector2 pos)
	{
		pos = pos.Clamp(new Vector2(Minimum, Minimum), new Vector2(Maximum - 1, Maximum - 1));
		static int map(float x) => (int)((x - Minimum) / (Maximum - Minimum) * BoardSize);
		return new Vector2I(map(pos.X), map(pos.Y));
	}

	public static Vector2 ConvertFromCell(Vector2I cell)
	{
		static float unmap(int x) => 2 + Minimum + x * cellSize + cellSize / 2;
		return new Vector2(unmap(cell.X), unmap(cell.Y));
	}
}
