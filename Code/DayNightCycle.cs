using Godot;
using System;

public class DayNightCycle : Node
{
  float timePassed = 0f;
  Godot.Environment sunset;
  Godot.Environment night;
  Godot.Environment day;

  int countdown = 10;	
  
	float dayStart = 0.35f;
	float dayEnd = 0.85f;
	float timeOfDay;
	public static float dayLength = 600;
  float sunsetEnd = 0.82f;
  float sunsetMid = 0.75f;	
  float sunsetStart = 0.65f;

  bool active = true;

	public float GetTimePassedToday()
	{
		return timePassed;
  }

  public override void _Ready()
  {
		timeOfDay = dayStart;

		day = (Godot.Environment)GD.Load("res://Lighting/Daytime.tres");
		sunset = (Godot.Environment)GD.Load("res://Lighting/Sunset2.tres");
		night = (Godot.Environment)GD.Load("res://Lighting/Gloomy.tres");

		UpdateEnvironment(day,night,0f);
  }

  public override void _Process(float delta)
  {
	if (active)
	{
	  timeOfDay += ((dayEnd-dayStart)/dayLength)*delta;
		timePassed += delta;

		float sunsetStartPercent = (-sunsetStart + timeOfDay) / (sunsetMid - sunsetStart);
	  sunsetStartPercent = Math.Min(sunsetStartPercent, 1);
	  sunsetStartPercent = Math.Max(sunsetStartPercent, 0);

	  float sunsetEndPercent = (-sunsetMid + timeOfDay) / (sunsetEnd - sunsetMid);
	  sunsetEndPercent = Math.Min(sunsetEndPercent, 1);
	  sunsetEndPercent = Math.Max(sunsetEndPercent, 0);

	  if ((sunsetStartPercent != 0 && sunsetStartPercent != 1) || (sunsetEndPercent != 0 && sunsetEndPercent != 1))
	  {
		if (sunsetStartPercent != 0 && sunsetStartPercent != 1)
		{
		  UpdateEnvironment(day, sunset, sunsetStartPercent);
		}
		else
		{
		  UpdateEnvironment(sunset, night, sunsetEndPercent);
		}
	  }
	  GetParent().GetNode("Sky").Call("set_time_of_day", timeOfDay);
	}
  }

  public void UpdateEnvironment(Godot.Environment start, Godot.Environment end, float percent)
  {
	var environment = GetParent().GetNode("Sky").GetNode("WorldEnvironment") as WorldEnvironment;

	environment.Environment.FogColor = new Color(start.FogColor.r * (1 - percent) + end.FogColor.r * percent
	, start.FogColor.g * (1 - percent) + end.FogColor.g * percent
	, start.FogColor.b * (1 - percent) + end.FogColor.b * percent);

	environment.Environment.FogSunColor = new Color(start.FogSunColor.r * (1 - percent) + end.FogSunColor.r * percent
	, start.FogSunColor.g * (1 - percent) + end.FogSunColor.g * percent
	, start.FogSunColor.b * (1 - percent) + end.FogSunColor.b * percent);

	environment.Environment.FogSunAmount = 0;

	environment.Environment.FogDepthBegin = start.FogDepthBegin * (1 - percent) + end.FogDepthBegin * percent;
	environment.Environment.FogDepthEnd = start.FogDepthEnd * (1 - percent) + end.FogDepthEnd * percent;
  }
}
