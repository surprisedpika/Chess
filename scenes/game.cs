using System.Collections.Generic;
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

	private Vector2I selectedCell;
	private List<Vector2I> legalMoves;

	private enum GameState
	{
		WhiteSelectPiece, WhiteSelectMove, BlackSelectPiece, BlackSelectMove, Done
	}
	private GameState gameState;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		highlighter = (CellHighlight)FindChild("CellHighlight", false);
		board = (Board)FindChild("Board", false);

		selectedCell = new(-1, -1);
		gameState = GameState.WhiteSelectPiece;

		board.ResetBoard();
		DrawBoard();
	}

	public override void _Input(InputEvent @event)
	{
		if (!@event.IsActionPressed("primary"))
		{
			return;
		}

		Vector2I mouseCell = ConvertToCell(GetViewport().GetMousePosition());

		switch (gameState)
		{
			case GameState.WhiteSelectPiece:
			case GameState.BlackSelectPiece:
				{
					if (board.GetPiece(mouseCell) is not Piece piece)
					{
						break;
					}
					bool whiteToPlay = gameState == GameState.WhiteSelectPiece;
					if (whiteToPlay == piece.isWhite)
					{
						selectedCell = mouseCell;
						legalMoves = board.GetLegalMoves(mouseCell);
						gameState = whiteToPlay ? GameState.WhiteSelectMove : GameState.BlackSelectMove;
						break;
					}
					break;
				}
			case GameState.WhiteSelectMove:
				{
					if (legalMoves.Contains(mouseCell))
					{
						board.MakeMove(selectedCell, mouseCell);
						DrawBoard();
						gameState = GameState.BlackSelectPiece;
						break;
					}
					if (mouseCell.Equals(selectedCell))
					{
						gameState = GameState.WhiteSelectPiece;
						break;
					}
					if (board.GetPiece(mouseCell) is not Piece mousePiece)
					{
						break;
					}
					if (mousePiece.isWhite)
					{
						selectedCell = mouseCell;
						legalMoves = board.GetLegalMoves(mouseCell);
						gameState = GameState.WhiteSelectMove;
						break;
					}
					break;
				}
			case GameState.BlackSelectMove:
				{
					if (legalMoves.Contains(mouseCell))
					{
						board.MakeMove(selectedCell, mouseCell);
						DrawBoard();
						gameState = GameState.WhiteSelectPiece;
						break;
					}
					if (mouseCell.Equals(selectedCell))
					{
						gameState = GameState.BlackSelectPiece;
						break;
					}
					if (board.GetPiece(mouseCell) is not Piece mousePiece)
					{
						break;
					}
					if (!mousePiece.isWhite)
					{
						selectedCell = mouseCell;
						legalMoves = board.GetLegalMoves(mouseCell);
						gameState = GameState.BlackSelectMove;
						break;
					}
					break;
				}
			default:
				break;
		}

		// Vector2I mouseCell = ConvertToCell(GetViewport().GetMousePosition());

		// List<Vector2I> legalMoves = board.GetLegalMoves(mouseCell);

		// foreach (Vector2I cell in legalMoves)
		// {
		// 	GD.Print(cell);
		// }
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
				if (board.GetPiece(cell) is not Piece piece)
				{
					continue;
				}

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
