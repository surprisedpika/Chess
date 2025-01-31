using System;
using Godot;

public partial class game : Node2D
{
	public const int Minimum = 50;
	public const int Maximum = 950;
	public const int BoardSize = 8;

	public const float cellSize = (Maximum - Minimum) / BoardSize;
	private CellHighlight highlighter;
	private Vector2I previousMouseCell;
	private static readonly Texture2D TestTexture = ResourceLoader.Load<Texture2D>("res://assets/pixel chess_v1.2/16x32 pieces/B_Knight.png");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		highlighter = (CellHighlight)FindChild("CellHighlight", false);
		previousMouseCell = new(-1, -1);

		for (int i = 0; i < 8; i++)
		{
			Sprite2D testPiece = new()
			{
				Texture = TestTexture,
				Centered = true,
				Scale = new(4f, 4f),
				Position = ConvertFromCell(new(i, i), new(16, 32), new(4f, 4f)),
				ZIndex = 100
			};
			AddChild(testPiece);
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2I currentMouseCell = ConvertToCell(GetViewport().GetMousePosition());
		if (currentMouseCell.Equals(previousMouseCell))
		{
			return;
		}
		highlighter.DeselectCell(previousMouseCell);
		highlighter.SelectCell(currentMouseCell);
		previousMouseCell = currentMouseCell;
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

	public static Vector2 ConvertFromCell(Vector2I cell, Vector2I size, Vector2 scale)
	{
		static float unmapValue(int location, int size, float scale)
		{
			float a = 1;
			a *= location;
			a *= cellSize;
			// do NOT question why you gotta take the 4th root here because I DO NOT KNOW
			a *= (float)Math.Pow(scale, 1 / 4);
			a += Minimum;
			a += cellSize / 2;
			return a + 2;
		}

		return new Vector2(unmapValue(cell.X, size.X, scale.X), unmapValue(cell.Y, size.Y, scale.Y));
	}
}
