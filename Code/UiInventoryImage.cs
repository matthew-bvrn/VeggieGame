using Godot;
using System;

public class UiInventoryImage : UiIndexedObject
{
  public void UpdateImage(int index, int quantity, string itemName)
  {
    if (index == m_index)
    {
      if (quantity > 0)
      {
        int y = ((PlantManager)GetTree().CurrentScene.GetNode("PlantManager")).PlantIndexFromFruitNameLookup(itemName);
        ((AtlasTexture)Texture).Region = new Rect2(0, y * 64, 64, 64);
      }
      else
      {
        ((AtlasTexture)Texture).Region = new Rect2(0, 0, 0, 0);
      }
    }
  }
}
