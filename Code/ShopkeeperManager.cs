using Godot;
using System;

public class ShopkeeperManager : Spatial
{
  VisualInstance selectionBox;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
	selectionBox = GetNode("Selection") as VisualInstance;
  }


  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
	Vector3 characterTranslation = ((Spatial)GetTree().CurrentScene.GetNode("Character")).GlobalTransform.origin;
	float distanceToChar = characterTranslation.DistanceTo(GlobalTransform.origin);
	if (distanceToChar < 3)
	{
	  if (selectionBox.Visible == false)
	  {
		selectionBox.Visible = true;
	  }

	  if (Input.IsActionJustReleased("action"))
	  {
		if (!((ShopManager)GetNode("ShopManager")).GetIsActive())
		{
		  ((ShopManager)GetNode("ShopManager")).OpenShop();
		}
	  }
	}
	else
	{
	  if (selectionBox.Visible == true)
	  {
		selectionBox.Visible = false;
	  }
	}
  }

}
