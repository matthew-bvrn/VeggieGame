using Godot;
using System;

public class MarketShopSpot : Spatial
{
  Inventory.InventorySlot m_heldItem = new Inventory.InventorySlot(3);

  float m_height;
  float m_offset = 0;
  float m_cycle = 0;
  Spatial m_sprite;

  public override void _Ready()
  {
    m_sprite = ((Spatial)GetNode("Sprite"));
    m_height = m_sprite.Translation.y;
  }

  public override void _Process(float delta)
  {
    if (m_heldItem.GetQuantity() > 0)
    {
      m_cycle += delta / 0.5f;
      if (m_cycle > 2 * Math.PI)
      {
        m_cycle -= 2 * (float)Math.PI;
      }

      m_offset = 0.12f * (float)Math.Cos(m_cycle);
      m_sprite.Translation = new Vector3(m_sprite.Translation.x, m_height + m_offset, m_sprite.Translation.z);
    }
  }

  public void SetItem(Inventory.InventoryItem item)
  {
    m_heldItem.SetItem(item);
    m_heldItem.EmitSignal(nameof(Inventory.InventorySlot.InventoryChanged), 3, GetQuantity(), m_heldItem.GetItemName());
    int y = ((PlantManager)GetTree().CurrentScene.GetNode("PlantManager")).PlantIndexFromFruitNameLookup(item.m_name);
    Texture texture = ((Sprite3D)GetNode("Sprite")).Texture;
    ((AtlasTexture)texture).Region = new Rect2(0, y * 64, 64, 64);
  }

  public void ChangeQuantity(int changeDelta)
  {
    m_heldItem.ChangeQuantity(changeDelta);
    m_heldItem.EmitSignal(nameof(Inventory.InventorySlot.InventoryChanged), 3, GetQuantity(), m_heldItem.GetItemName());
    if (m_heldItem.GetQuantity() == 0)
    {
      ((Sprite3D)GetNode("Sprite")).Visible = false;
    }
    else
    {
      ((Sprite3D)GetNode("Sprite")).Visible = true;
    }
  }

  public int GetQuantity()
  {
    return m_heldItem.GetQuantity();
  }

  public string GetItemName()
  {
    return m_heldItem.GetItemName();
  }

  public void SetUpConnections(UiNumber uiNumber, UiInventoryImage uiInventoryImage)
  {
    m_heldItem.Connect(nameof(Inventory.InventorySlot.InventoryChanged), uiNumber, nameof(UiNumber.UpdateValue));
    m_heldItem.Connect(nameof(Inventory.InventorySlot.InventoryChanged), uiInventoryImage, nameof(UiInventoryImage.UpdateImage));
    m_heldItem.EmitSignal(nameof(Inventory.InventorySlot.InventoryChanged), 3, 0, new Inventory.InventorySlot());
  }
}
