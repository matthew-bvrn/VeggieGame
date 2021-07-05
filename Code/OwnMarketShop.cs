using Godot;
using System;

public class OwnMarketShop : MarketShop
{
	DateTime m_shopClosedTime = DateTime.MinValue;

	public OwnMarketShop(MarketShopManager _parent) : base(_parent)
	{
	}

	public override void Update(MarketShopSpot marketShopSpot)
	{
		if(Input.IsActionJustReleased("action") && m_shopClosedTime.AddSeconds(0.1f).CompareTo(DateTime.Now) < 0)
		{
			UiMenuMarketManager marketManager = m_parent.GetTree().CurrentScene.GetNode("UI").GetNode("Market").GetNode("Root") as UiMenuMarketManager;
			if (!marketManager.GetIsActive())
			{
				Character character = ((Character)m_parent.GetTree().CurrentScene.GetNode("Character")) as Character;
				character.m_state = Character.State.Shop;
				marketManager.SetIsActive(true);
				marketManager.Init(this, marketShopSpot);
			}
		}
	}
	
	public void AddStock(int inventoryIndex)
	{
		Inventory characterInventory = ((Spatial)m_parent.GetTree().CurrentScene.GetNode("Character")).GetNode("Inventory") as Inventory;
		Inventory.InventoryItem heldItem = new Inventory.InventoryItem(characterInventory.GetItemName(inventoryIndex), characterInventory.GetItemType(inventoryIndex));
		int heldItemQuantity = characterInventory.GetSelectedItemQuantity();

		if (heldItemQuantity > 0 && heldItem.m_type == "Fruit")
		{if(m_parent.GetClosestSpot().GetQuantity() == 0 || heldItem.m_name == m_parent.GetClosestSpot().GetItemName())
		{
			m_parent.GetClosestSpot().SetItem(heldItem);
			m_parent.GetClosestSpot().ChangeQuantity(1);

			characterInventory.ChangeSlot(-1, inventoryIndex);
		}}
	}

	public void RemoveStock()
	{
		//todo
  }

	public void CloseShop()
	{
		m_shopClosedTime = DateTime.Now;
  }
}
