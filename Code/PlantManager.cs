using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlantManager : Node
{
	public class PlantType
	{
		public float growthTime {get; set;}
		public string fruitName {get; set;}
		public string seedName {get; set;}
	} 

	public Dictionary<string, PlantType> plants;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var file = new File();
		file.Open("res://Data/PlantData.json", File.ModeFlags.Read);
		string content = file.GetAsText();
		file.Close();
		plants = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,PlantType>>(content);
	}
	
	public string SeedLookup(string seed)
	{
		foreach(KeyValuePair<string, PlantType> plant in plants)
		{
			if(plant.Value.seedName == seed)
			{
				return plant.Key;
			}
		}
		//GD.Assert(false, "Seedlookup failed: seeds with this name don't exist");
		
		return "";
	}

}
