using Godot;
using System;

public class Plant : Spatial
{
	public float totalGrowthTime = 0;
	public float currentGrowthTime = 0;
	public int growthStage = 0;
	public bool bHarvestable = false;
	public string plantName;

	public override void _Ready()
	{

	}


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
	if(bHarvestable == false)
	{
	currentGrowthTime += delta;
	float ratio = currentGrowthTime / totalGrowthTime;
	growthStage = (int)Math.Min(7,Math.Floor(ratio * 8));
	((Sprite3D)GetChildren()[0]).RegionRect = new Rect2(growthStage*32, 0, 32, 32);
	if(growthStage==7)
	{
		bHarvestable = true;
	}
	}
	
  }
}
