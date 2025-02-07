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

	public override void _Ready()
	{
		// Get references to children that we need to use
		highlighter = (CellHighlight)FindChild("CellHighlight", false);
		board = (Board)FindChild("Board", false);

		// Reset private variables
		selectedCell = new(-1, -1);
		gameState = GameState.WhiteSelectPiece;

		// Set up board and draw it
		board.ResetBoard();
		DrawBoard();
	}

	public override void _Input(InputEvent @event)
	{
		if (!@event.IsActionPressed("primary"))
		{
			return;
		}

		// A cell has been clicked on

		Vector2I mouseCell = ConvertToCell(GetViewport().GetMousePosition());

		switch (gameState)
		{
			case GameState.WhiteSelectPiece:
			case GameState.BlackSelectPiece:
				{
					// Selecting the piece to move
					if (board.GetPiece(mouseCell) is not Piece piece)
					{
						break;
					}
					// Cell selected is a piece
					bool whiteToPlay = gameState == GameState.WhiteSelectPiece;
					if (whiteToPlay != piece.isWhite)
					{
						break;
					}
					// Piece selected is the right colour
					selectedCell = mouseCell;
					legalMoves = board.GetLegalMoves(mouseCell);
					gameState = whiteToPlay ? GameState.WhiteSelectMove : GameState.BlackSelectMove;
					// Get list of legal moves for that piece and store it, change game state
					break;
				}
			case GameState.WhiteSelectMove:
			case GameState.BlackSelectMove:
				{
					// Selecting the cell to move the piece to
					bool whiteToPlay = gameState == GameState.WhiteSelectMove;
					if (legalMoves.Contains(mouseCell))
					{
						// If a legal move was selected, make it
						board.MakeMove(selectedCell, mouseCell);
						DrawBoard();
						gameState = whiteToPlay ? GameState.BlackSelectPiece : GameState.WhiteSelectPiece;
						break;
					}
					if (mouseCell.Equals(selectedCell))
					{
						// If the player clicks on the piece again, deselect it
						gameState = whiteToPlay ? GameState.WhiteSelectPiece : GameState.BlackSelectPiece;
						break;
					}
					if (board.GetPiece(mouseCell) is not Piece mousePiece)
					{
						// If the player clicks on some random square, try again
						break;
					}
					if (mousePiece.isWhite != whiteToPlay)
					{
						break;
					}
					// If the player clicks on a different piece of the same colour, select it instead
					selectedCell = mouseCell;
					legalMoves = board.GetLegalMoves(mouseCell);
					gameState = whiteToPlay ? GameState.WhiteSelectMove : GameState.BlackSelectMove;
					break;
				}
			default:
				break;
		}
	}

	public void DrawBoard()
	{
		foreach (var child in GetChildren())
		{
			if (child.Equals(board) || child.Equals(highlighter))
			{
				continue;
			}
			// Remove any old pieces
			child.QueueFree();
		}
		// For each cell on the board...
		for (int y = 0; y < BoardSize; y++)
		{
			for (int x = 0; x < BoardSize; x++)
			{
				Vector2I cell = new(x, y);
				// If the cell is empty, ignore it
				if (board.GetPiece(cell) is not Piece piece)
				{
					continue;
				}

				// Create the piece at the cell
				Sprite2D sprite = new()
				{
					Texture = piece.GetTexture(),
					Centered = true,
					Scale = new(4f, 4f),
					Position = ConvertFromCell(cell),
					ZIndex = 100,
					Offset = new(0, -3)
				};
				// Add the piece into the scene
				AddChild(sprite);
			}
		}
	}

	// Given some position in screenspace, return the cell it is in.
	public static Vector2I ConvertToCell(Vector2 pos)
	{
		// Clamp mouse position to within bounds of board
		pos = pos.Clamp(new Vector2(Minimum, Minimum), new Vector2(Maximum - 1, Maximum - 1));
		// Map each dimension to a grid cell
		static int map(float x) => (int)((x - Minimum) / (Maximum - Minimum) * BoardSize);
		return new Vector2I(map(pos.X), map(pos.Y));
	}

	// Return position in screenspace of the center of a grid cell
	public static Vector2 ConvertFromCell(Vector2I cell)
	{
		static float unmap(int x) => 2 + Minimum + x * cellSize + cellSize / 2;
		return new Vector2(unmap(cell.X), unmap(cell.Y));
	}
}
