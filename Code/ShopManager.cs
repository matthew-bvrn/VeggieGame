using Godot;
using System;
using System.Collections.Generic;

public class ShopManager : Node
{
  [Signal]
  public delegate void ShopChanged();


  public class ShopItem
  {
	public Inventory.InventoryItem item;
	public int price;
	public int quantity;

	public ShopItem()
	{
	  item = new Inventory.InventoryItem();
	  price = 0;
	  quantity = 0;
	}

	public ShopItem(Inventory.InventoryItem _item, int _price, int _quantity)
	{
	  item = _item;
	  price = _price;
	  quantity = _quantity;
	}

	public void changeValue(int change)
	{
	  quantity += change;
	  if (quantity == 0)
	  {
		price = 0;
		quantity = 0;
		item = new Inventory.InventoryItem();
	  }
	}
  }

  bool isActive = false;

  public bool GetIsActive()
  {
	return isActive;
  }

  public List<ShopItem> itemsForSale = new List<ShopItem>();
  int selected = 0;
  int slots = 3;
  Character character = new Character();

  public override void _Ready()
  {
	character = ((Character)GetTree().CurrentScene.GetNode("Character"));

	itemsForSale.Add(new ShopItem(new Inventory.InventoryItem("Starplant Seeds", "Seeds"), 5, 3));
	itemsForSale.Add(new ShopItem(new Inventory.InventoryItem("Starplant Seeds", "Seeds"), 10, 4));
	itemsForSale.Add(new ShopItem(new Inventory.InventoryItem("Starplant Seeds", "Seeds"), 15, 5));

	foreach (Node childNode in GetTree().CurrentScene.GetNode("UI").GetNode("Shop").GetChildren())
	{
	  if (childNode.Name.Contains("Panel"))
	  {
		Connect(nameof(ShopChanged), ((ShopText)childNode.GetNode("Label")), nameof(ShopText.onShopChanged));
	  }
	}

	EmitSignal(nameof(ShopChanged), this);
  }

  public void OpenShop()
  {
	GD.Print("Shop opened");
	isActive = true;
	selected = 0;
	character.m_state = Character.State.Shop;
	foreach (Node childNode in GetTree().CurrentScene.GetNode("UI").GetNode("Shop").GetChildren())
	{
	  if (childNode.Name.Contains("Panel") || childNode.Name.Contains("Descriptions"))
	  {
		if (((CanvasItem)childNode).Visible == false)
		{
		  ((CanvasItem)childNode).Visible = true;
		}
	  }

	}
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
	if (isActive)
	{
	  if (Input.IsActionJustPressed("ui_end"))
	  {
		character.m_state = Character.State.Active;
		isActive = false;

		foreach (Node childNode in GetTree().CurrentScene.GetNode("UI").GetNode("Shop").GetChildren())
		{
		  if (childNode.Name.Contains("Panel") || childNode.Name.Contains("Descriptions"))
		  {
			if (((CanvasItem)childNode).Visible == true)
			{
			  ((CanvasItem)childNode).Visible = false;
			}
		  }

		}
	  }

	  if (Input.IsActionJustPressed("ui_right"))
	  {
		selected++;
		selected = selected % slots;
		UpdatePanels();
	  }
	  else if (Input.IsActionJustPressed("ui_left"))
	  {
		selected--;
		if (selected < 0)
		{
		  selected += slots;
		}
		UpdatePanels();
	  }
	  else if (Input.IsActionJustPressed("action"))
	  {
		BuySelected();
	  }

	}
  }

  void BuySelected()
  {
	Inventory inventory = ((Inventory)character.GetNode("Inventory"));
	if (itemsForSale[selected].quantity > 0)
	{
	  if (inventory.money > itemsForSale[selected].price)
	  {
		int index = (inventory.GetFreeSlotIndex(itemsForSale[selected].item.m_name));
		if (index > -1)
		{
		  inventory.money -= itemsForSale[selected].price;
		  inventory.ChangeSlot(1, index, itemsForSale[selected].item);
		  itemsForSale[selected].changeValue(-1);
		  EmitSignal(nameof(ShopChanged), this);
		  GD.Print("Bought ", itemsForSale[selected].item.m_name, " for £", itemsForSale[selected].price, ", money left £", inventory.money);
		}
		else
		{
		  GD.Print("No free slots");
		}
	  }
	  else
	  {
		GD.Print("Not enough money");
	  }
	}
  }

  public void UpdatePanels()
  {
	((Selection)GetTree().CurrentScene.GetNode("UI").GetNode("Shop").GetNode("Selection")).UpdatePanels(selected);
  }
}
