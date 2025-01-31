using System;
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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		highlighter = (CellHighlight)FindChild("CellHighlight", false);

		for (int i = 0; i < 8; i++)
		{
			Sprite2D testPiece = new()
			{
				Texture = new Piece(false, Type.King).getTexture(),
				Centered = true,
				Scale = new(4f, 4f),
				Position = ConvertFromCell(new(i, i), 4f),
				ZIndex = 100,
				Offset = new(0, -3)
			};
			AddChild(testPiece);
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

	public static Vector2 ConvertFromCell(Vector2I cell, float scale)
	{
		static float unmapValue(int location, float scale)
		{
			float a = location * cellSize;
			// do NOT question why you gotta take the 4th root here because I DO NOT KNOW
			float b = a * (float)Math.Pow(scale, 1 / 4);
			b += Minimum;
			b += cellSize / 2;
			b += 2;
			return b;
		}

		return new Vector2(unmapValue(cell.X, scale), unmapValue(cell.Y, scale));
	}
}
