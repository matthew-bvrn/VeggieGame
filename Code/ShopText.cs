using Godot;
using System;

public class ShopText : Label
{
	int slot;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		slot = ((InventorySlotPanel)GetParent()).index;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public void onShopChanged(Node node)
  {
		ShopManager.ShopItem shopSlot =  ((ShopManager)node).itemsForSale[slot];
	  Text = shopSlot.item.m_name;
		if(shopSlot.item.m_name!="Empty")
		{
			Text+=" "+shopSlot.quantity	+" £"+shopSlot.price;
		}
  }
}
