using Godot;
using System;
using System.Collections.Generic;

public class Inventory : Node
{

  public class InventoryItem
  {
    public string m_name;
    public string m_type;

    public InventoryItem()
    {
      m_name = "Empty";
      m_type = "None";
    }

    public InventoryItem(string _item, string _type)
    {
      m_name = _item;
      m_type = _type;
    }
  }

  public class InventorySlot : Node
  {
    [Signal]
    public delegate void InventoryChanged(int index, int quantity, string itemName);

    InventoryItem m_item;
    int m_index;
    int m_quantity;

    public InventorySlot()
    {
      m_item = new InventoryItem();
      m_quantity = 0;
      m_index = 0;
      EmitSignal(nameof(InventoryChanged), m_index, GetQuantity(), m_item.m_name);
    }

    public InventorySlot(int _index)
    {
      m_item = new InventoryItem();
      m_quantity = 0;
      m_index = _index;
      EmitSignal(nameof(InventoryChanged), m_index, GetQuantity(), m_item.m_name);
    }

    public InventorySlot(InventoryItem _item, int _quantity, int _index)
    {
      m_item = _item;
      m_quantity = _quantity;
      m_index = _index;
      EmitSignal(nameof(InventoryChanged), m_index, GetQuantity(), m_item.m_name);
    }

    public int GetQuantity()
    {
      return m_quantity;
    }

    public string GetItemName()
    {
      return m_item.m_name;
    }

    public string GetItemType()
    {
      return m_item.m_type;
    }

    public void SetItem(InventoryItem item)
    {
      m_item = item;
      EmitSignal(nameof(InventoryChanged), m_index, GetQuantity(), m_item.m_name);
    }

    public void ChangeQuantity(int changeDelta)
    {
      m_quantity += changeDelta;
      if (m_quantity <= 0)
      {
        m_quantity = 0;
        m_item = new Inventory.InventoryItem();
      }
      EmitSignal(nameof(InventoryChanged), m_index, GetQuantity(), m_item.m_name);
    }
  }

  List<InventorySlot> contents = new List<InventorySlot>();
  int m_selected = 0;
  int slots = 3;
  public int money = 9999;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    //Temp debug code
    for (int i = 0; i < 3; i++)
    {
      InventorySlot slot;
      if (i == 0)
      {
        slot = new InventorySlot(new InventoryItem("Starplant Seeds", "Seeds"), 5, i);
      }
      else if (i == 1)
      {
        slot = new InventorySlot(new InventoryItem("Lustlilies", "Fruit"), 6, i);
      }
      else
      {
        slot = new InventorySlot(new InventoryItem("Starfruit", "Fruit"), 7, i);
      }

      contents.Add(slot);
    }

    int index = 0;
    foreach (Node childNode in GetTree().CurrentScene.GetNode("UI").GetNode("Inventory").GetChildren())
    {
      if (childNode.Name.Contains("Panel"))
      {
        contents[index].Connect(nameof(InventorySlot.InventoryChanged), ((Node)childNode.GetChildren()[0]), nameof(InventoryText.onInventoryChanged));
        contents[index].EmitSignal(nameof(InventorySlot.InventoryChanged), index, contents[index].GetQuantity(), contents[index].GetItemName());

        index++;
      }
    }
  }

  //  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
    if (Input.IsActionJustReleased("scroll_down"))
    {
      m_selected++;
      m_selected = m_selected % slots;
      UpdatePanels();
    }
    else if (Input.IsActionJustReleased("scroll_up"))
    {
      m_selected--;
      if (m_selected < 0)
      {
        m_selected += slots;
      }
      UpdatePanels();
    }
  }

  public void UpdatePanels()
  {
    ((Selection)GetTree().CurrentScene.GetNode("UI").GetNode("Inventory").GetNode("Selection")).UpdatePanels(m_selected);
  }

  public InventorySlot GetSlot(int index)
  {
    return contents[index];
  }

  public int GetFreeSlotIndex(string itemName)
  {
    int index = 0;
    foreach (var slot in contents)
    {
      if (slot.GetItemName() == itemName)
      {
        return index;
      }
      index++;
    }
    index = 0;
    foreach (var slot in contents)
    {
      if (slot.GetItemName() == "Empty")
      {
        return index;
      }
      index++;
    }
    return -1;
  }

  public void ChangeSelectedSlot(int change)
  {
    ChangeSlot(change, m_selected);
  }

  public void ChangeSelectedSlot(int change, InventoryItem item)
  {
    ChangeSlot(change, m_selected, item);
  }

  public void ChangeSlot(int change, int index)
  {
    contents[index].ChangeQuantity(change);
  }

  public void ChangeSlot(int change, int index, InventoryItem item)
  {
    contents[index].SetItem(item);
    ChangeSlot(change, index);
  }

  public int GetSelectedItemQuantity()
  {
    return GetItemQuantity(m_selected);
  }

  public int GetItemQuantity(int index)
  {
    return contents[index].GetQuantity();
  }

  public string GetSelectedItemName()
  {
    return contents[m_selected].GetItemName();
  }

  public string GetItemName(int index)
  {
    return contents[index].GetItemName();
  }

  public string GetSelectedItemType()
  {
    return contents[m_selected].GetItemType();
  }

  public string GetItemType(int index)
  {
    return contents[index].GetItemType();
  }
}
