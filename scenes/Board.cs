using Godot;
using System;
using System.Collections.Generic;

public partial class Board : Sprite2D
{

	public enum Type
	{
		King, Queen, Pawn, Rook, Bishop, Knight
	}

	private static readonly Dictionary<Piece, Texture2D> PieceTextures = new()
	{
		{new (false, Type.King), ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/B_King.png")},
		{new (false, Type.Queen), ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/B_Queen.png")},
		{new (false, Type.Pawn), ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/B_Pawn.png")},
		{new (false, Type.Rook), ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/B_Rook.png")},
		{new (false, Type.Bishop), ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/B_Bishop.png")},
		{new (false, Type.Knight), ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/B_Knight.png")},
		{new (true, Type.King), ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/W_King.png")},
		{new (true, Type.Queen), ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/W_Queen.png")},
		{new (true, Type.Pawn), ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/W_Pawn.png")},
		{new (true, Type.Rook), ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/W_Rook.png")},
		{new (true, Type.Bishop), ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/W_Bishop.png")},
		{new (true, Type.Knight), ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/W_Knight.png")}
	};

	private readonly List<Piece?> Pieces = new(64);

	public struct Piece
	{
		public bool isWhite;
		public Type type;

		public Piece(bool colorIsWhite, Type pieceType)
		{
			isWhite = colorIsWhite;
			type = pieceType;
		}

		public readonly Texture2D GetTexture()
		{
			return PieceTextures.GetValueOrDefault(this);
		}
	}

	// Get the piece at a cell. If the cell is empty, return null
	public Piece? GetPiece(Vector2I cell)
	{
		if (!IsInBoard(cell))
		{
			throw new IndexOutOfRangeException();
		}
		// Convert a grid coordinate to an index from 0-63
		int index = cell.X + cell.Y * 8;
		return Pieces[index];
	}

	// Set a cell to a piece or null
	public void SetCell(Vector2I cell, Piece? piece)
	{
		if (!IsInBoard(cell))
		{
			throw new IndexOutOfRangeException();
		}
		// Convert a grid coordinate to an index from 0-63
		int index = cell.X + cell.Y * 8;
		Pieces[index] = piece;
	}

	// Put a piece on a new cell, and remove it from the old cell
	public void MakeMove(Vector2I from, Vector2I to)
	{
		// If either cell is outside the board, throw an error
		if (!IsInBoard(from) || !IsInBoard(to))
		{
			throw new IndexOutOfRangeException();
		}
		// If there is no piece to move, throw an error
		if (GetPiece(from) is not Piece piece)
		{
			throw new Exception("No piece at cell");
		}
		// Remove the piece from the old cell
		SetCell(from, null);
		// Place it at the new cell
		SetCell(to, piece);
	}

	// Return a list of legal moves for a given piece
	public List<Vector2I> GetLegalMoves(Vector2I cell)
	{
		List<Vector2I> moves = new();
		// If the piece is not a piece, return an empty list
		if (GetPiece(cell) is not Piece piece)
		{
			return moves;
		}
		switch (piece.type)
		{
			// If the piece is a pawn...
			case Type.Pawn:
				{
					// TODO: Captures and en-passant
					GD.Print("Pawn Selected");
					Vector2I potentialPawnMove;
					if (piece.isWhite)
					{
						// First potential move is one move up (if white)
						// 0,0 is at the top left of the board
						potentialPawnMove = new(cell.X, cell.Y - 1);
					}
					else
					{
						// Or one move down (if black)
						potentialPawnMove = new(cell.X, cell.Y + 1);
					}
					// Pawns cannot move into or through another piece
					if (GetPiece(potentialPawnMove) is not null)
					{
						break;
					}
					// Add the first move
					moves.Add(potentialPawnMove);

					// If the pawn hasn't moved, it can move 2 squares
					// This logic comes after as the pawn can only move 2 squares in a subset of the cases it can move 1 square
					if (piece.isWhite && cell.Y == 6)
					{
						potentialPawnMove = new(cell.X, cell.Y - 2);
					}
					else if (!piece.isWhite && cell.Y == 1)
					{
						potentialPawnMove = new(cell.X, cell.Y + 2);
					}
					else
					{
						// Otherwise, there are no more legal moves
						break;
					}
					if (GetPiece(potentialPawnMove) is not null)
					{
						// Pawns can't move into another piece
						break;
					}
					// Add the second move
					moves.Add(potentialPawnMove);
					break;
				}
			case Type.Knight:
				{
					GD.Print("Knight Selected");
					// Knights move in an L shape, 2 in one direction, 1 in the other
					List<Vector2I> potentialKnightMoves = new()
				{
					new Vector2I(cell.X - 1, cell.Y + 2),
					new Vector2I(cell.X - 1, cell.Y - 2),

					new Vector2I(cell.X + 1, cell.Y + 2),
					new Vector2I(cell.X + 1, cell.Y - 2),

					new Vector2I(cell.X - 2, cell.Y + 1),
					new Vector2I(cell.X - 2, cell.Y - 1),

					new Vector2I(cell.X + 2, cell.Y + 1),
					new Vector2I(cell.X + 2, cell.Y - 1)
				};
					foreach (Vector2I potentialKnightMove in potentialKnightMoves)
					{
						// The knight cannot move outside the board
						if (!IsInBoard(potentialKnightMove))
						{
							continue;
						}
						// Check if there is a piece where the knight is moving
						if (GetPiece(potentialKnightMove) is Piece blocker)
						{
							// If the pieces are the same colour, the move is illegal
							if (piece.isWhite == blocker.isWhite)
							{
								continue;
							}
							// If they are different colour, the knight can take the piece
							moves.Add(potentialKnightMove);
						}
						else
						{
							// The knight is free to move to an empty square
							moves.Add(potentialKnightMove);
						}
					}
					break;
				}
			case Type.Rook:
				{
					GD.Print("Rook Selected");
					var finished = false;

					void checkMove(Vector2I potentialMove)
					{
						if (!IsInBoard(potentialMove))
						{
							finished = true;
							return;
						}
						if (GetPiece(potentialMove) is Piece blocker)
						{
							if (piece.isWhite != blocker.isWhite)
							{
								moves.Add(potentialMove);
							}
							finished = true;
							return;
						}
						moves.Add(potentialMove);
					}

					// Check moves to the right
					for (int i = cell.X + 1; i < Game.BoardSize; i++)
					{
						if (finished)
						{
							continue;
						}
						Vector2I potentialMove = new(i, cell.Y);
						checkMove(potentialMove);
					}
					finished = false;

					// Check moves to the left
					for (int i = cell.X - 1; i > 0; i--)
					{
						if (finished)
						{
							continue;
						}
						Vector2I potentialMove = new(i, cell.Y);
						checkMove(potentialMove);
					}
					finished = false;

					// Check moves above
					for (int i = cell.Y - 1; i > 0; i--)
					{
						if (finished)
						{
							continue;
						}
						Vector2I potentialMove = new(cell.X, i);
						checkMove(potentialMove);
					}
					finished = false;

					// Check moves below
					for (int i = cell.Y + 1; i < Game.BoardSize; i++)
					{
						if (finished)
						{
							continue;
						}
						Vector2I potentialMove = new(cell.X, i);
						checkMove(potentialMove);
					}
					finished = false;

					break;
				}
			default:
				break;
		}
		return moves;
	}

	// Checks a cell is inside a board
	public static bool IsInBoard(Vector2I cell)
	{
		if (cell.X < 0 || cell.X > Game.BoardSize - 1 || cell.Y < 0 || cell.Y > Game.BoardSize - 1)
		{
			return false;
		}
		return true;
	}

	// Set up the pieces on the board
	public void ResetBoard()
	{
		// Clear board
		for (int i = 0; i < 64; i++)
		{
			Pieces[0] = null;
		}

		// Put one pawn in each column for black and white
		for (int i = 0; i < Game.BoardSize; i++)
		{
			SetCell(new(i, 1), new(false, Type.Pawn));
			SetCell(new(i, 6), new(true, Type.Pawn));
		}

		// Set up other pieces as normal
		SetCell(new(0, 0), new(false, Type.Rook));
		SetCell(new(0, 7), new(true, Type.Rook));

		SetCell(new(1, 0), new(false, Type.Knight));
		SetCell(new(1, 7), new(true, Type.Knight));

		SetCell(new(2, 0), new(false, Type.Bishop));
		SetCell(new(2, 7), new(true, Type.Bishop));

		SetCell(new(3, 0), new(false, Type.Queen));
		SetCell(new(3, 7), new(true, Type.Queen));

		SetCell(new(4, 0), new(false, Type.King));
		SetCell(new(4, 7), new(true, Type.King));

		SetCell(new(5, 0), new(false, Type.Bishop));
		SetCell(new(5, 7), new(true, Type.Bishop));

		SetCell(new(6, 0), new(false, Type.Knight));
		SetCell(new(6, 7), new(true, Type.Knight));

		SetCell(new(7, 0), new(false, Type.Rook));
		SetCell(new(7, 7), new(true, Type.Rook));
	}

	public override void _Ready()
	{
		for (int i = 0; i < 64; i++)
		{
			Pieces.Add(null);
		}
	}
}
