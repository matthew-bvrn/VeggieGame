using Godot;
using System;
using System.Collections.Generic;

public class PlantSpotManager : Spatial
{
	VisualInstance selectionBox;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		selectionBox = GetNode("Selection") as VisualInstance;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
 {
	
	Vector3 characterTranslation = ((Spatial)GetTree().CurrentScene.GetNode("Character")).Translation;
	float distanceToChar =  characterTranslation.DistanceTo(Translation);
	if(distanceToChar<7)
	{
		if(selectionBox.Visible == false)
		{
			selectionBox.Visible = true;
		}
		
		Node closestSpot = ((Node)GetChildren()[0]);
		float closestDistance = 1000000;
		foreach(Node node in GetChildren())
		{
			float distance = (((Spatial)node).GlobalTransform.origin).DistanceTo(characterTranslation);
			if(distance<closestDistance)
			{
				closestDistance = distance;
				closestSpot = node;
			}
		}
		selectionBox.Translation = ((Spatial)closestSpot).Translation;

		
		if(Input.IsActionJustPressed("action"))
		{
			Node plantSpot = closestSpot;
			Character character = ((Character)GetTree().CurrentScene.GetNode("Character"));
			Inventory inventory = ((Inventory)character.GetNode("Inventory"));
			Inventory.InventorySlot slot = inventory.contents[inventory.selected];
			
			if(((string)plantSpot.Get("ownedPlantName") == "Empty"))
			{
				if(slot.item.type == "Seeds" && slot.quantity>0)
				{
					string plantName = ((PlantManager)GetTree().CurrentScene.GetNode("PlantManager")).SeedLookup(slot.item.name);
					inventory.ChangeSlot(-1, inventory.selected);
					plantSpot.Call("plant", plantName);
					GD.Print("Planted seeds");
				}
			}
			else
			{
				Node plantnode = ((Node)(plantSpot.GetChildren()[1]));
				Plant plant = plantnode as Plant;
				string fruitName = ((PlantManager)GetTree().CurrentScene.GetNode("PlantManager")).plants[(string)plantSpot.Get("ownedPlantName")].fruitName;
				int index = inventory.GetFreeSlotIndex(fruitName);
				if(plant.bHarvestable && index!=-1)
				{
					inventory.ChangeSlot(1, index, new Inventory.InventoryItem(fruitName, "Fruit"));
					plantSpot.Call("harvest");
					GD.Print("Harvested plant");
				}
				else
				{
					if(!plant.bHarvestable)
					{
						GD.Print("Couldn't harvest: Plant not yet grown");
					}
					else
					{
						GD.Print("Couldn't harvest: Slot contains something else");
					}
				}
			}
		}
		
	}
	else
	{
		if(selectionBox.Visible == true)
		{
			selectionBox.Visible = false;
		}
	}
 }
}
