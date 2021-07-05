using Godot;
using System;

public class LightFlicker : Spatial
{
	float previousIntensity;
	float currentIntensity = 2.0f;
	float targetIntensity;
	float timeTotal;
	float timeCurrent;
	
	
	void NewIntensity()
	{
		var rand = new Random();
		previousIntensity = currentIntensity;
		targetIntensity = (float)(2.2+rand.NextDouble()/8);
		timeTotal = (float)((Math.Abs(targetIntensity - previousIntensity)));
		timeCurrent = 0;
	}
	
	// Called when the node enters the scene tree for the first time.d
	public override void _Ready()
	{
		NewIntensity();
	}

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
	Light light = FindNode("OmniLight") as Light;
	
	timeCurrent += delta;	
	var intensityChange = Math.Abs(previousIntensity - targetIntensity) * (timeCurrent/timeTotal);
	
	if(previousIntensity < targetIntensity)
	{
		currentIntensity = previousIntensity + intensityChange;
	}
	else
	{
		currentIntensity = previousIntensity - intensityChange;
	}
	
	light.LightEnergy = currentIntensity;

	if(timeCurrent > timeTotal)
	{
		NewIntensity();
	}
	
  }
}
