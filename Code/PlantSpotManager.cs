using Godot;

public class PlantSpotManager : Spatial
{
  InteractableLocator m_interactableLocator;

  public override void _Ready()
  {
	m_interactableLocator = new InteractableLocator(this, GetNode("Selection") as VisualInstance, 7, true);
  }

  public override void _Process(float delta)
  {

	Node closestNode = m_interactableLocator.GetNearestNode();
	if (closestNode != null)
	{
	  if (Input.IsActionJustPressed("action"))
	  {
		DoAction(closestNode);
	  }
	}
  }

  void DoAction(Node plantSpot)
  {
	Character character = ((Character)GetTree().CurrentScene.GetNode("Character"));
	Inventory inventory = ((Inventory)character.GetNode("Inventory"));

	if (((string)plantSpot.Get("ownedPlantName") == "Empty"))
	{
	  if (inventory.GetSelectedItemType() == "Seeds" && inventory.GetSelectedItemQuantity() > 0)
	  {
		string plantName = ((PlantManager)GetTree().CurrentScene.GetNode("PlantManager")).SeedLookup(inventory.GetSelectedItemName());
		inventory.ChangeSelectedSlot(-1);
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
	  if (plant.bHarvestable && index != -1)
	  {
		inventory.ChangeSlot(1, index, new Inventory.InventoryItem(fruitName, "Fruit"));
		plantSpot.Call("Harvest");
		GD.Print("Harvested plant");
	  }
	  else
	  {
		if (!plant.bHarvestable)
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
