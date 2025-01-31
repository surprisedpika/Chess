using Godot;
using System.Collections.Generic;

public partial class Board : Sprite2D
{

	public enum Type
	{
		King, Queen, Pawn, Rook, Bishop, Knight
	}

	private static readonly Dictionary<Type, Texture2D> BlackTextures = new()
	{
		{Type.King, ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/B_King.png")},
		{Type.Queen, ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/B_Queen.png")},
		{Type.Pawn, ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/B_Pawn.png")},
		{Type.Rook, ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/B_Rook.png")},
		{Type.Bishop, ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/B_Bishop.png")},
		{Type.Knight, ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/B_Knight.png")}
	};

	private static readonly Dictionary<Type, Texture2D> WhiteTextures = new()
	{
		{Type.King, ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/W_King.png")},
		{Type.Queen, ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/W_Queen.png")},
		{Type.Pawn, ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/W_Pawn.png")},
		{Type.Rook, ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/W_Rook.png")},
		{Type.Bishop, ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/W_Bishop.png")},
		{Type.Knight, ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/W_Knight.png")},
	};

	public class Piece
	{
		public bool isWhite;
		public Type type;

		public Piece(bool colorIsWhite, Type pieceType)
		{
			isWhite = colorIsWhite;
			type = pieceType;
		}

		public Texture2D getTexture()
		{
			if (isWhite)
			{
				return WhiteTextures.GetValueOrDefault(type);
			}
			return BlackTextures.GetValueOrDefault(type);
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
