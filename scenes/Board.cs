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

	public Piece? GetPiece(Vector2I cell)
	{
		if (!IsInBoard(cell))
		{
			throw new IndexOutOfRangeException();
		}
		int index = cell.X + cell.Y * 8;
		return Pieces[index];
	}

	public void SetCell(Vector2I cell, Piece? piece)
	{
		if (!IsInBoard(cell))
		{
			throw new IndexOutOfRangeException();
		}
		int index = cell.X + cell.Y * 8;
		Pieces[index] = piece;
	}

	public void MakeMove(Vector2I from, Vector2I to)
	{
		if (!IsInBoard(from) || !IsInBoard(to))
		{
			throw new IndexOutOfRangeException();
		}
		if (GetPiece(from) is not Piece piece)
		{
			throw new Exception("No piece at cell");
		}
		SetCell(from, null);
		SetCell(to, piece);
	}

	public List<Vector2I> GetLegalMoves(Vector2I cell)
	{
		List<Vector2I> moves = new();
		if (GetPiece(cell) is not Piece piece)
		{
			return moves;
		}
		switch (piece.type)
		{
			case Type.Pawn:
				GD.Print("Pawn Selected");
				Vector2I potentialPawnMove;
				if (piece.isWhite)
				{
					potentialPawnMove = new(cell.X, cell.Y - 1);
				}
				else
				{
					potentialPawnMove = new(cell.X, cell.Y + 1);
				}
				if (GetPiece(potentialPawnMove) is not null)
				{
					break;
				}
				moves.Add(potentialPawnMove);
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
					break;
				}
				if (GetPiece(potentialPawnMove) is not null)
				{
					break;
				}
				moves.Add(potentialPawnMove);
				break;
			case Type.Knight:
				GD.Print("Knight Selected");
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
					if (!IsInBoard(potentialKnightMove))
					{
						continue;
					}
					if (GetPiece(potentialKnightMove) is Piece blocker)
					{
						if (piece.isWhite == blocker.isWhite)
						{
							continue;
						}
						moves.Add(potentialKnightMove);
					}
					else
					{
						moves.Add(potentialKnightMove);
					}
				}
				break;
			default:
				break;
		}
		return moves;
	}

	public static bool IsInBoard(Vector2I cell)
	{
		if (cell.X < 0 || cell.X > Game.BoardSize - 1 || cell.Y < 0 || cell.Y > Game.BoardSize - 1)
		{
			return false;
		}
		return true;
	}

	public void ResetBoard()
	{
		for (int i = 0; i < Game.BoardSize; i++)
		{
			SetCell(new(i, 1), new(false, Type.Pawn));
			SetCell(new(i, 6), new(true, Type.Pawn));
		}
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
