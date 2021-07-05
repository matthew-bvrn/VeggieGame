using Godot;
using System;

public class ShopkeeperManager : Spatial
{
  InteractableLocator interactableLocator;

  public override void _Ready()
  {
	interactableLocator = new InteractableLocator(this, GetNode("Selection") as VisualInstance, 3, false);
  }

  public override void _Process(float delta)
  {
	Node closest = interactableLocator.GetNearestNode();

	if (closest != null && Input.IsActionJustReleased("action") && !((ShopManager)GetNode("ShopManager")).GetIsActive())
	{
	  ((ShopManager)GetNode("ShopManager")).OpenShop();
	}
  }

}
