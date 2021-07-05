using Godot;
using System;

public abstract class MarketShop
{
  protected MarketShopManager m_parent;

  public MarketShop(MarketShopManager _parent)
  {
	  m_parent = _parent;
  }

  public Node GetParent()
  {
	return m_parent;
  }

  public abstract void Update(MarketShopSpot marketShopSpot);
}
