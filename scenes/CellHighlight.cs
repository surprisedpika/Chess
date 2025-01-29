using System;
using System.Collections.Generic;
using Godot;
using static game;

public partial class CellHighlight : Sprite2D
{
	private Dictionary<Vector2I, Sprite2D> children;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		children = new Dictionary<Vector2I, Sprite2D>();
	}

	public void SelectCell(Vector2I cell)
	{
		GD.Print("Selecting:", cell);
		Sprite2D highlight = new()
		{
			Texture = Texture,
			Position = ConvertFromCell(cell)
		};
		AddChild(highlight);
		children.Add(cell, highlight);
	}

	public bool DeselectCell(Vector2I cell)
	{
		throw new NotImplementedException();
	}

	public void DeselectAllCells()
	{
		foreach (KeyValuePair<Vector2I, Sprite2D> cell in children)
		{
			GD.Print(cell.Value.Position);
			// DeselectCell(cell.Key);
		}
	}
}
