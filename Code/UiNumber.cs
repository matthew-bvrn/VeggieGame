using Godot;
using System;

public class UiNumber : Node
{
  int m_value = 0;
  [Export]
  public int m_index;

  public override void _Ready()
  {
    SetValue(0);
  }

  public void UpdateValue(int index, int value, string name)
  {
    if (m_index == index)
    {
      SetValue(value);
    }
  }

  void SetValue(int value)
  {
    m_value = value;
    ((AtlasTexture)((TextureRect)GetNode("1s")).Texture).Region = new Rect2((value % 10) * 5, 0, 5, 9);
    if (m_value > 9)
    {
      ((TextureRect)GetNode("10s")).Visible = true;
      ((AtlasTexture)((TextureRect)GetNode("10s")).Texture).Region = new Rect2(((value / 10) % 10) * 5, 0, 5, 9);
    }
    else
    {
      ((TextureRect)GetNode("10s")).Visible = false;
      ((TextureRect)GetNode("100s")).Visible = false;
    }
    if (m_value > 99)
    {
      ((TextureRect)GetNode("100s")).Visible = true;
      ((AtlasTexture)((TextureRect)GetNode("100s")).Texture).Region = new Rect2(((value / 100) % 10) * 5, 0, 5, 9);
    }
    else
    {
      ((TextureRect)GetNode("100s")).Visible = false;
    }
  }
}
