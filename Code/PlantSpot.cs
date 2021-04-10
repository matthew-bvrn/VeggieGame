using Godot;
using System;


public class PlantSpot : Spatial
{	
	public string ownedPlantName = "Empty";
	
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void plant(string name)
	{
		ownedPlantName = name;
		var scene = GD.Load<PackedScene>("res://ObjectsAbstract/Plant.tscn");

		PlantManager.PlantType plant = ((PlantManager)GetTree().CurrentScene.GetNode("PlantManager")).plants[ownedPlantName];
		float totalGrowthTime = plant.growthTime;
		int spriteIndex = plant.index;
		
		//Randomise the growth time a bit
		var rng = new RandomNumberGenerator();
		rng.Randomize();
		float randomOffset = rng.RandfRange(-0.1f,0.1f);
		totalGrowthTime = totalGrowthTime + randomOffset*totalGrowthTime;
		
		var spatialNode = scene.Instance();
		
		spatialNode.Set("totalGrowthTime", totalGrowthTime);
		spatialNode.Set("plantName", ownedPlantName);
		spatialNode.Set("spriteIndex", spriteIndex);

		AddChild(spatialNode);
		
	}
	
	public void harvest()
	{
		ownedPlantName = "Empty";
		((Node)GetChildren()[1]).QueueFree();
	}
	
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
