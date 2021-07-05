using Godot;
using System;

public class MarketShopManager : Spatial
{
	[Export]
	bool m_isOwnShop = false;
	MarketShop m_shop;
	InteractableLocator m_interactableLocator;
	MarketShopSpot m_closestSpot;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(m_isOwnShop)
		{
			m_shop = new OwnMarketShop(this);
		}
		m_interactableLocator = new InteractableLocator(this, GetNode("Selection") as VisualInstance, 10, true);
	}

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
		m_closestSpot = m_interactableLocator.GetNearestNode() as MarketShopSpot;
		if (m_closestSpot != null)
		{
			m_shop.Update(m_closestSpot);
		}
  }

	public MarketShopSpot GetClosestSpot()
	{
		return m_closestSpot;
  }
}
