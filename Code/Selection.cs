using Godot;
using System;

public class Selection : Node
{
	StyleBoxFlat StyleDefault = GD.Load<StyleBoxFlat>("res://UIDefault.tres");
	StyleBoxFlat StyleHighlighted = GD.Load<StyleBoxFlat>("res://UIHighlighted.tres");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UpdatePanels(0);
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {

  }

public void UpdatePanels(int selected)	
{
foreach(Node child in GetParent().GetChildren())
{
	if(child.Name.Contains("Panel"))
	{
		Panel control = ((Panel)child);
		if(((InventorySlotPanel)child).index == selected)
		{
			control.Set("custom_styles/panel",StyleHighlighted);
		}
		else
		{
			control.Set("custom_styles/panel",StyleDefault);
		}
	}
}
}
}
