using Godot;
using System;

public class InventoryText : Label
{
	int slot;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		slot = ((InventorySlotPanel)GetParent()).index;
		onInventoryChanged();
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public void onInventoryChanged()
  {
		Inventory inventory = GetTree().CurrentScene.GetNode("Character").GetNode("Inventory") as Inventory;
	  Text = inventory.contents[slot].item.name;
		if(Text!="Empty")
		{
			Text+=" "+inventory.contents[slot].quantity;
		}
  }
}
