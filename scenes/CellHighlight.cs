using System.Collections.Generic;
using Godot;
using static Game;

public partial class CellHighlight : Sprite2D
{
	private Dictionary<Vector2I, Sprite2D> children;
	private Vector2I previousMouseCell;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		children = new Dictionary<Vector2I, Sprite2D>();
		previousMouseCell = new(-1, -1);
	}

	public override void _Process(double delta)
	{
		Vector2I currentMouseCell = ConvertToCell(GetViewport().GetMousePosition());
		if (currentMouseCell.Equals(previousMouseCell))
		{
			return;
		}

		DeselectCell(previousMouseCell);
		SelectCell(currentMouseCell);
		previousMouseCell = currentMouseCell;
	}

	public bool IsSelected(Vector2I cell)
	{
		return children.ContainsKey(cell);
	}

	public void SelectCell(Vector2I cell)
	{
		if (IsSelected(cell))
		{
			return;
		}
		Sprite2D highlight = new()
		{
			Texture = Texture,
			Centered = true,
			Position = ConvertFromCell(cell),
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
