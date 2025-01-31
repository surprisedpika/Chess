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
		Sprite2D highlight = new()
		{
			Texture = Texture,
			Centered = true,
			Position = ConvertFromCell(cell, new(32, 32), new(1.75f, 1.75f)),
			Scale = new(3.5f, 3.5f),
			ZIndex = 10
		};
		AddChild(highlight);
		children.Add(cell, highlight);
	}

	public bool DeselectCell(Vector2I cell)
	{
		if (children.TryGetValue(cell, out Sprite2D instance))
		{
			children.Remove(cell);
			instance.QueueFree();
			return true;
		}
		else
		{
			return false;
		}
	}

	public void DeselectAllCells()
	{
		foreach (KeyValuePair<Vector2I, Sprite2D> cell in children)
		{
			DeselectCell(cell.Key);
		}
	}
}
