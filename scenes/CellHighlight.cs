using System.Collections.Generic;
using Godot;
using static Game;

public partial class CellHighlight : Sprite2D
{
	private Dictionary<Vector2I, Sprite2D> children;
	private Vector2I previousMouseCell;

	public override void _Ready()
	{
		// Initialise variables
		children = new Dictionary<Vector2I, Sprite2D>();
		previousMouseCell = new(-1, -1);
	}

	public override void _Process(double delta)
	{
		// Get the cell the mouse is hovering on
		Vector2I currentMouseCell = ConvertToCell(GetViewport().GetMousePosition());
		// If it is the same cell as last frame, don't do anything
		if (currentMouseCell.Equals(previousMouseCell))
		{
			return;
		}

		// Remove the highlight from the cell the mouse was previously on
		DeselectCell(previousMouseCell);
		// Highlight this new cell
		SelectCell(currentMouseCell);
		previousMouseCell = currentMouseCell;
	}

	public bool IsSelected(Vector2I cell)
	{
		return children.ContainsKey(cell);
	}

	// Draw a highlight on a given cell
	public void SelectCell(Vector2I cell)
	{
		if (IsSelected(cell))
		{
			return;
		}
		// Create highlight sprite and place it on the cell
		Sprite2D highlight = new()
		{
			Texture = Texture,
			Centered = true,
			Position = ConvertFromCell(cell),
			Scale = new(3.5f, 3.5f),
			ZIndex = 10
		};
		// Add the highlight to the scene
		AddChild(highlight);
		// Make a record of the highlight
		children.Add(cell, highlight);
	}

	// Remove the highlight from a given cell
	public bool DeselectCell(Vector2I cell)
	{
		if (children.TryGetValue(cell, out Sprite2D instance))
		{
			// Delete the highlight and record to it
			children.Remove(cell);
			instance.QueueFree();
			return true;
		}
		else
		{
			// The cell was never highlighted in the first place 
			return false;
		}
	}


	// Delete every highlight
	public void DeselectAllCells()
	{
		foreach (KeyValuePair<Vector2I, Sprite2D> cell in children)
		{
			DeselectCell(cell.Key);
		}
	}
}
