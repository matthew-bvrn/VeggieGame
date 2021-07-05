using Godot;
using System;

public class InventoryText : Label
{
  [Export]
  int m_index;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  { }

  //  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public void onInventoryChanged(int index, int quantity, string name)
  {
	if (index == m_index)
	{
	  Text = name;
	  if (Text != "Empty")
	  {
		Text += " " + quantity;
	  }
	}
  }
}
