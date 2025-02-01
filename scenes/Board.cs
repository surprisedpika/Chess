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
		if (cell.X < 0 || cell.X > Game.BoardSize - 1 || cell.Y < 0 || cell.Y > Game.BoardSize - 1)
		{
			throw new IndexOutOfRangeException();
		}
		int index = cell.X + cell.Y * 8;
		return Pieces[index];
	}

	public void SetPiece(Vector2I cell, Piece piece)
	{
		if (cell.X < 0 || cell.X > Game.BoardSize - 1 || cell.Y < 0 || cell.Y > Game.BoardSize - 1)
		{
			throw new IndexOutOfRangeException();
		}
		int index = cell.X + cell.Y * 8;
		Pieces[index] = piece;
	}

	public void ResetBoard()
	{
		for (int i = 0; i < Game.BoardSize; i++)
		{
			SetPiece(new(i, 1), new(false, Type.Pawn));
			SetPiece(new(i, 6), new(true, Type.Pawn));
		}
		SetPiece(new(0, 0), new(false, Type.Rook));
		SetPiece(new(0, 7), new(true, Type.Rook));

		SetPiece(new(1, 0), new(false, Type.Knight));
		SetPiece(new(1, 7), new(true, Type.Knight));

		SetPiece(new(2, 0), new(false, Type.Bishop));
		SetPiece(new(2, 7), new(true, Type.Bishop));

		SetPiece(new(3, 0), new(false, Type.Queen));
		SetPiece(new(3, 7), new(true, Type.Queen));

		SetPiece(new(4, 0), new(false, Type.King));
		SetPiece(new(4, 7), new(true, Type.King));

		SetPiece(new(5, 0), new(false, Type.Bishop));
		SetPiece(new(5, 7), new(true, Type.Bishop));

		SetPiece(new(6, 0), new(false, Type.Knight));
		SetPiece(new(6, 7), new(true, Type.Knight));

		SetPiece(new(7, 0), new(false, Type.Rook));
		SetPiece(new(7, 7), new(true, Type.Rook));
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for (int i = 0; i < 64; i++)
		{
			Pieces.Add(null);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
