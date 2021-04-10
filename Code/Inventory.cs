using Godot;
using System;
using System.Collections.Generic;

public class Inventory : Node
{

  [Signal]
  public delegate void InventoryChanged();

  public class InventoryItem
  {
    public string name;
    public string type;

    public InventoryItem()
    {
      name = "Empty";
      type = "None";
    }

    public InventoryItem(string _item, string _type)
    {
      name = _item;
      type = _type;
    }
  }

  public class InventorySlot
  {
    public InventoryItem item;
    public int quantity;

    public InventorySlot()
    {
      item = new InventoryItem();
      quantity = 0;
    }

    public InventorySlot(InventoryItem _item, int _quantity)
    {
      item = _item;
      quantity = _quantity;
    }

    public void changeValue(int value)
    {
      quantity += value;
      if (quantity == 0)
      {
        quantity = 0;
        item = new Inventory.InventoryItem();
      }
    }
  }

  public List<InventorySlot> contents = new List<InventorySlot>();
  public int selected = 0;
  int slots = 3;
  public int money = 9999;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    foreach (Node childNode in GetTree().CurrentScene.GetNode("UI").GetNode("Inventory").GetChildren())
    {
      if (childNode.Name.Contains("Panel"))
      {
        Connect(nameof(InventoryChanged), ((Node)childNode.GetChildren()[0]), nameof(InventoryText.onInventoryChanged));
      }
    }

    for (int i = 0; i < 3; i++)
    {
      InventorySlot slot;
      if (i == 0)
      {
        slot = new InventorySlot(new InventoryItem("Starplant Seeds", "Seeds"), 5);
      }
      else if (i == 1)
      {
        slot = new InventorySlot(new InventoryItem("Lustlily Pods", "Seeds"), 5);
      }
      else
      {
        slot = new InventorySlot();
      }
      contents.Add(slot);
    }
  }

  //  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
    if (Input.IsActionJustReleased("scroll_down"))
    {
      selected++;
      selected = selected % slots;
      UpdatePanels();
    }
    else if (Input.IsActionJustReleased("scroll_up"))
    {
      selected--;
      if (selected < 0)
      {
        selected += slots;
      }
      UpdatePanels();
    }
  }

  public void UpdatePanels()
  {
    ((Selection)GetTree().CurrentScene.GetNode("UI").GetNode("Inventory").GetNode("Selection")).UpdatePanels(selected);
  }

  public int GetFreeSlotIndex(string itemName)
  {
    int index = 0;
    foreach (var slot in contents)
    {
      if (slot.item.name == itemName)
      {
        return index;
      }
      index++;
    }
    index = 0;
    foreach (var slot in contents)
    {
      if (slot.item.name == "Empty")
      {
        return index;
      }
      index++;
    }
    return -1;
  }

  public void ChangeSlot(int change, int index)
  {
    contents[index].changeValue(change);
    EmitSignal(nameof(InventoryChanged));
  }

  public void ChangeSlot(int change, int index, InventoryItem item)
  {
    contents[index].item = item;
    ChangeSlot(change, index);
  }

}
