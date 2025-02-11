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


	// Convert a grid coordinate to an index from 0-63
	private static int GetIndexFromCell(Vector2I cell)
	{
		if (!IsInBoard(cell))
		{
			throw new IndexOutOfRangeException();
		}
		return cell.X + cell.Y * 8;
	}

	// Convert an index from 0-63 to a grid coordinate
	private static Vector2I GetCellFromIndex(int index)
	{
		if (index < 0 || index > 63)
		{
			throw new IndexOutOfRangeException();
		}
		return new(index % 8, index / 8);
	}

	// Get the piece at a cell. If the cell is empty, return null
	public Piece? GetPiece(Vector2I cell)
	{
		int index = GetIndexFromCell(cell);
		return Pieces[index];
	}

	// Set a cell to a piece or null
	public void SetCell(Vector2I cell, Piece? piece)
	{
		int index = GetIndexFromCell(cell);
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

		if (IsInCheck(true))
		{
			GD.Print("White is in check");
		}

		if (IsInCheck(false))
		{
			GD.Print("Black is in check");
		}
	}

	private List<Vector2I> GetPawnLegalMoves(Vector2I cell, bool isWhite)
	{
		// TODO: en-passant & promotion
		List<Vector2I> moves = new();

		// First, check captures
		List<Vector2I> potentialCaptures = new()
		{
			// Forward right
			new(cell.X + 1, isWhite ? cell.Y - 1 : cell.Y + 1),
			// Forward Left
			new(cell.X - 1, isWhite ? cell.Y - 1 : cell.Y + 1),
		};

		foreach (var potentialCapture in potentialCaptures)
		{
			if (
				// If the pawn is on the edge, can only capture one way
				!IsInBoard(potentialCapture) ||
				// Can't capture an empty cell
				GetPiece(potentialCapture) is not Piece capturable ||
				// Can't capture a piece of the same colour
				capturable.isWhite == isWhite
			)
			{
				continue;
			}
			moves.Add(potentialCapture);
		}

		// First normal potential move is one cell forward
		Vector2I potentialMove = new(cell.X, isWhite ? cell.Y - 1 : cell.Y + 1);

		// Can't move outside board and...
		// Can't move into or through another piece
		if (!IsInBoard(potentialMove) || GetPiece(potentialMove) is not null)
		{
			return moves;
		}

		// Add the first move
		moves.Add(potentialMove);

		// If the pawn hasn't moved, it can move 2 squares
		// This logic comes after as the pawn can only move 2 squares in a subset of the cases it can move 1 square

		if ((isWhite && cell.Y == 6) || (!isWhite && cell.Y == 1))
		{
			potentialMove = new(cell.X, isWhite ? cell.Y - 2 : cell.Y + 2);
		}
		else
		{
			// Otherwise, there are no more legal moves
			return moves;
		}

		// Can't move into or through another piece
		if (GetPiece(potentialMove) is not null)
		{
			// Pawns can't move into another piece
			return moves;
		}
		// Add the second move
		moves.Add(potentialMove);
		return moves;
	}

	private List<Vector2I> GetKnightLegalMoves(Vector2I cell, bool isWhite)
	{
		List<Vector2I> moves = new();
		List<Vector2I> potentialMoves = new()
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
		foreach (Vector2I potentialMove in potentialMoves)
		{
			// The knight cannot move outside the board
			if (!IsInBoard(potentialMove))
			{
				continue;
			}
			// Check if there is a piece where the knight is moving
			if (GetPiece(potentialMove) is Piece blocker)
			{
				// If the pieces are the same colour, the move is illegal
				if (isWhite == blocker.isWhite)
				{
					continue;
				}
				// If they are different colour, the knight can take the piece
				moves.Add(potentialMove);
			}
			else
			{
				// The knight is free to move to an empty square
				moves.Add(potentialMove);
			}
		}
		return moves;
	}

	private List<Vector2I> GetKingLegalMoves(Vector2I cell, bool isWhite)
	{
		// TODO: castling
		List<Vector2I> moves = new();
		// For the column before, same column, and column after...
		for (int x = -1; x <= 1; x++)
		{
			// For the row before, same row, and row after...
			for (int y = -1; y <= 1; y++)
			{
				Vector2I potentialMove = new(cell.X + x, cell.Y + y);
				GD.Print(potentialMove);
				// Can't move to a square outside the board
				if (!IsInBoard(potentialMove))
				{
					continue;
				}
				// Can't move to the square we already are
				if (x == y && x == 0)
				{
					continue;
				}
				// If there is a piece in the square we are trying to move to...
				if (GetPiece(potentialMove) is Piece piece)
				{
					// If the piece is the opposite colour to us, we can take it
					if (piece.isWhite != isWhite)
					{
						moves.Add(potentialMove);
					}
					// If it's the same colour, we can't take it
					continue;
				}
				// If the square we are trying to move to is empty, we can go there
				moves.Add(potentialMove);
			}
		}
		return moves;
	}

	// Rook, queen, bishop
	private List<Vector2I> GetSlidingPieceLegalMoves(Vector2I cell, bool isWhite, List<Vector2I> directions)
	{
		List<Vector2I> moves = new();
		List<Vector2I> blockedDirections = new();

		// For the size of the board
		for (int i = 1; i <= 7; i++)
		{
			// For each direction the piece moves
			foreach (Vector2I direction in directions)
			{
				// Don't move past a blockage (edge of board, piece)
				if (blockedDirections.Contains(direction))
				{
					continue;
				}

				Vector2I potentialCell = cell + direction * i;
				// If the next move is off the edge of the board, mark that direction as blocked
				if (!IsInBoard(potentialCell))
				{
					blockedDirections.Add(direction);
					continue;
				}

				// If the next move is into an empty cell, thats fine
				if (GetPiece(potentialCell) is not Piece blocker)
				{
					moves.Add(potentialCell);
					continue;
				}

				// If the next move is into a cell occupied by a piece of the same colour, that direction is blocked
				if (blocker.isWhite == isWhite)
				{
					blockedDirections.Add(direction);
					continue;
				}

				// The next cell is into a piece of the opposite colour, which we can take, but can't move further
				moves.Add(potentialCell);
				blockedDirections.Add(direction);
			}

			// If every direction is blocked, we are finished.
			if (blockedDirections.Count == directions.Count)
			{
				return moves;
			}
		}

		return moves;
	}

	// Return a list of legal moves for a given piece
	public List<Vector2I> GetLegalMoves(Vector2I cell)
	{
		List<Vector2I> moves = new();
		// If the cell is empty, return an empty list
		if (GetPiece(cell) is not Piece piece)
		{
			return moves;
		}
		switch (piece.type)
		{
			//TODO: Checks and checkmates
			case Type.Pawn:
				{
					GD.Print("Pawn Selected");
					moves.AddRange(GetPawnLegalMoves(cell, piece.isWhite));
					break;
				}
			case Type.Knight:
				{
					GD.Print("Knight Selected");
					moves.AddRange(GetKnightLegalMoves(cell, piece.isWhite));
					break;
				}
			case Type.Queen:
				{
					GD.Print("Queen Selected");
					List<Vector2I> directions = new()
					{
						Vector2I.Up,
						Vector2I.Down,
						Vector2I.Left,
						Vector2I.Right,
						new(-1, -1), // Up left
						new(-1, 1), // Up right
						new(1, -1), // Down left
						new(1, 1) // Down right
					};
					moves.AddRange(GetSlidingPieceLegalMoves(cell, piece.isWhite, directions));
					break;
				}
			case Type.Rook:
				{
					GD.Print("Rook Selected");
					List<Vector2I> directions = new()
					{
						Vector2I.Up,
						Vector2I.Down,
						Vector2I.Left,
						Vector2I.Right
					};
					moves.AddRange(GetSlidingPieceLegalMoves(cell, piece.isWhite, directions));
					break;
				}
			case Type.Bishop:
				{
					GD.Print("Bishop Selected");
					List<Vector2I> directions = new()
					{
						new(-1, -1), // Up left
						new(-1, 1), // Up right
						new(1, -1), // Down left
						new(1, 1) // Down right
					};
					moves.AddRange(GetSlidingPieceLegalMoves(cell, piece.isWhite, directions));
					break;
				}
			case Type.King:
				{
					GD.Print("King Selected");
					moves.AddRange(GetKingLegalMoves(cell, piece.isWhite));
					break;
				}
			default:
				throw new Exception("Unknown piece type found!");
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

	public bool IsInCheck(bool white)
	{
		// Find the king
		var index = Pieces.FindIndex(p => p != null && p.Value.isWhite == white && p.Value.type == Type.King);
		if (index == -1)
		{
			throw new Exception("King not found");
		}
		var pos = GetCellFromIndex(index);

		// Check for knights
		foreach (var move in GetKnightLegalMoves(pos, white))
		{
			if (GetPiece(move) is not Piece piece)
			{
				continue;
			}
			if (piece.type == Type.Knight)
			{
				return true;
			}
		}

		// Check for bishops, rooks, queens

		// Pretend the king is a queen
		List<Vector2I> directions = new()
		{
			Vector2I.Up,
			Vector2I.Down,
			Vector2I.Left,
			Vector2I.Right,
			new(-1, -1), // Up left
			new(-1, 1), // Up right
			new(1, -1), // Down left
			new(1, 1) // Down right
		};

		List<Vector2I> blockedDirections = new();

		// Mostly reused from GetSlidingPieceLegalMoves
		for (int i = 1; i <= 7; i++)
		{
			// For all 8 directions a queen can move
			foreach (Vector2I direction in directions)
			{
				//
				if (blockedDirections.Contains(direction))
				{
					continue;
				}

				Vector2I potentialCell = pos + direction * i;

				if (!IsInBoard(potentialCell))
				{
					blockedDirections.Add(direction);
					continue;
				}

				if (GetPiece(potentialCell) is not Piece piece)
				{
					continue;
				}

				blockedDirections.Add(direction);

				if (piece.isWhite == white)
				{
					continue;
				}

				// If the piece is just one square away and a king (somehow)
				if (i == 1 && piece.type == Type.King)
				{
					// We are in check
					return true;
				}

				var directionAbs = direction.Abs();

				// If a piece of the opposite colour is a rook's move away...
				if (directionAbs.X == directionAbs.Y)
				{
					// If it's a rook or a queen...
					if (piece.type == Type.Rook || piece.type == Type.Queen)
					{
						// We are in check
						return true;
					}
					// Otherwise, that direction is blocked
					blockedDirections.Add(direction);
					continue;
				}

				// If a piece of the opposite colour is a bishop's move away...

				// If it is a bishop or queen...
				if (piece.type == Type.Bishop || piece.type == Type.Queen)
				{
					// We are in check
					return true;
				}

				// If the piece is just one diagonal square away...
				if (i == 1)
				{
					// And a pawn...
					if (piece.type == Type.Pawn)
					{
						// And we are in front of it...
						if (direction.Y == 1 == white)
						{
							// We are in check
							return true;
						}
					}
				}

				// Otherwise, that direction is blocked
				blockedDirections.Add(direction);
			}
			// If all directions are blocked and we haven't found a check...
			if (blockedDirections.Count == directions.Count)
			{
				// We aren't in check
				return false;
			}
		}

		// If we exhaust all possible checks, we aren't in check.
		return false;
	}

	public override void _Ready()
	{
		for (int i = 0; i < 64; i++)
		{
			Pieces.Add(null);
		}
	}
}
