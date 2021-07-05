using Godot;
using System;
using System.Collections.Generic;

public class UiMenuMarketManager : Control
{
  struct Button
  {
    public enum Type { None, InventorySlot, OtherSlot, Exit, ScrollNumber };

    public Button(int up, int down, int left, int right, Type type, UiMenuSelectableBase button)
    {
      m_up = up;
      m_down = down;
      m_left = left;
      m_right = right;
      m_button = button;
      m_type = type;
    }

    public int m_up;
    public int m_down;
    public int m_left;
    public int m_right;
    public UiMenuSelectableBase m_button;
    public Type m_type;
  }

  bool m_isActive = false;
  int m_selectedIndex = 0;
  List<Button> m_buttons = new List<Button>();
  List<UiNumber> m_uiNumbers = new List<UiNumber>();
  List<UiInventoryImage> m_uiInventoryImages = new List<UiInventoryImage>();
  OwnMarketShop m_marketShop;
  MarketShopSpot m_marketSpot;
  Inventory m_playerInventory;

  public override void _Ready()
  {
    //TODO do this with automatic lookups

    m_buttons.Add(new Button(-1, 3, -1, 1, Button.Type.InventorySlot, GetNode("Inv0") as UiMenuSelectableBase));
    m_buttons.Add(new Button(-1, 3, 0, 2, Button.Type.InventorySlot, GetNode("Inv1") as UiMenuSelectableBase));
    m_buttons.Add(new Button(-1, 4, 1, -1, Button.Type.InventorySlot, GetNode("Inv2") as UiMenuSelectableBase));
    m_buttons.Add(new Button(0, 5, -1, 4, Button.Type.OtherSlot, GetNode("Slot") as UiMenuSelectableBase));
    m_buttons.Add(new Button(2, 5, 3, -1, Button.Type.ScrollNumber, GetNode("PriceHighlight") as UiMenuSelectableBase));
    m_buttons.Add(new Button(4, -1, 3, -1, Button.Type.Exit, GetNode("OK") as UiMenuSelectableBase));

    m_uiNumbers.Add(GetNode("NumInv0") as UiNumber);
    m_uiNumbers.Add(GetNode("NumInv1") as UiNumber);
    m_uiNumbers.Add(GetNode("NumInv2") as UiNumber);
    m_uiNumbers.Add(GetNode("NumSlot") as UiNumber);

    m_uiInventoryImages.Add(GetNode("Inv0Pic") as UiInventoryImage);
    m_uiInventoryImages.Add(GetNode("Inv1Pic") as UiInventoryImage);
    m_uiInventoryImages.Add(GetNode("Inv2Pic") as UiInventoryImage);
    m_uiInventoryImages.Add(GetNode("SlotPic") as UiInventoryImage);

    m_buttons[0].m_button.SetState(UiMenuSelectableBase.State.Highlighted);
  }

  public void Init(OwnMarketShop marketShop, MarketShopSpot marketSpot)
  {
    m_marketShop = marketShop;
    m_marketSpot = marketSpot;
    m_playerInventory = marketShop.GetParent().GetTree().CurrentScene.GetNode("Character").GetNode("Inventory") as Inventory;

    //Set up connections and initialise
    for (int i = 0; i < 3; i++)
    {
      m_playerInventory.GetSlot(i).Connect(nameof(Inventory.InventorySlot.InventoryChanged), m_uiNumbers[i], nameof(UiNumber.UpdateValue));
      m_playerInventory.GetSlot(i).Connect(nameof(Inventory.InventorySlot.InventoryChanged), m_uiInventoryImages[i], nameof(UiInventoryImage.UpdateImage));
      m_playerInventory.GetSlot(i).EmitSignal(nameof(Inventory.InventorySlot.InventoryChanged), i, m_playerInventory.GetItemQuantity(i), m_playerInventory.GetItemName(i));
    }
    m_marketSpot.SetUpConnections(m_uiNumbers[3], m_uiInventoryImages[3]);

    m_selectedIndex = 0;
    m_buttons[0].m_button.SetState(UiMenuSelectableBase.State.Highlighted);
  }

  public void SetIsActive(bool isActive)
  {
    m_isActive = isActive;
    Visible = isActive ? true : false;
  }

  public bool GetIsActive()
  {
    return m_isActive;
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
    if (m_isActive)
    {
      if (Input.IsActionPressed("ui_end"))
      {
        CloseShop();
      }

      if (Input.IsActionJustPressed("ui_up") || Input.IsActionJustPressed("ui_down") || Input.IsActionJustPressed("ui_left") || Input.IsActionJustPressed("ui_right"))
      {
        int newSelected = -1;
        if (Input.IsActionJustPressed("ui_up"))
        {
          newSelected = m_buttons[m_selectedIndex].m_up;
        }
        else if (Input.IsActionJustPressed("ui_down"))
        {
          newSelected = m_buttons[m_selectedIndex].m_down;

        }
        else if (Input.IsActionJustPressed("ui_left"))
        {
          newSelected = m_buttons[m_selectedIndex].m_left;

        }
        else if (Input.IsActionJustPressed("ui_right"))
        {
          newSelected = m_buttons[m_selectedIndex].m_right;

        }

        if (newSelected != -1)
        {
          m_buttons[m_selectedIndex].m_button.SetState(UiMenuSelectableBase.State.Default);
          m_selectedIndex = newSelected;
          m_buttons[m_selectedIndex].m_button.SetState(UiMenuSelectableBase.State.Highlighted);
        }
      }

      if (Input.IsActionJustPressed("action"))
      {
        m_buttons[m_selectedIndex].m_button.SetState(UiMenuSelectableBase.State.Pressed);

        switch (m_buttons[m_selectedIndex].m_type)
        {
          case Button.Type.InventorySlot:
            m_marketShop.AddStock(m_selectedIndex);
            break;
          case Button.Type.OtherSlot:
            m_marketShop.RemoveStock();
            break;
          case Button.Type.Exit:
            CloseShop();
            break;
          default:
            break;
        }
      }
    }
  }

  private void CloseShop()
  {
    foreach (Button button in m_buttons)
    {
      button.m_button.SetState(UiMenuSelectableBase.State.Default);
    }

  ((Character)GetTree().CurrentScene.GetNode("Character")).m_state = Character.State.Active;
    m_marketShop.CloseShop();
    m_marketShop = null;
    SetIsActive(false);
  }

}
