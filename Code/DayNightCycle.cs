using Godot;
using System;

public class DayNightCycle : Node
{
  float transistionTime = 120f;
  float timePassed = 0f;
  Godot.Environment sunset;
  Godot.Environment night;

  ProceduralSky mySky;
  ProceduralSky sunsetSky;
  ProceduralSky nightSky;
  int countdown = 10;

  bool active = true;

  public override void _Ready()
  {
	sunset = (Godot.Environment)GD.Load("res://Lighting/Sunset2.tres");
	night = (Godot.Environment)GD.Load("res://Lighting/Gloomy.tres");

	//mySky = (ProceduralSky)Environment.BackgroundSky;
	//sunsetSky = (ProceduralSky)sunset.BackgroundSky;
	//	nightSky = (ProceduralSky)night.BackgroundSky;
  }

  public override void _Process(float delta)
  {
	if (active)
	{
	  timePassed += delta;
	  float percentage = Math.Min(1f, timePassed / transistionTime);

/*
	  countdown--;
	  /*
	  if (countdown == 0)
	  {
	  mySky.SkyTopColor = new Color(sunsetSky.SkyTopColor.r * (1 - percentage) + nightSky.SkyTopColor.r * percentage
		  , sunsetSky.SkyTopColor.g * (1 - percentage) + nightSky.SkyTopColor.g * percentage
		  , sunsetSky.SkyTopColor.b * (1 - percentage) + nightSky.SkyTopColor.b * percentage);

	  mySky.SkyHorizonColor = new Color(sunsetSky.SkyHorizonColor.r * (1 - percentage) + nightSky.SkyHorizonColor.r * percentage
		   , sunsetSky.SkyHorizonColor.g * (1 - percentage) + nightSky.SkyHorizonColor.g * percentage
		   , sunsetSky.SkyHorizonColor.b * (1 - percentage) + nightSky.SkyHorizonColor.b * percentage);

	  mySky.SkyCurve = sunsetSky.SkyCurve * (1 - percentage) + nightSky.SkyCurve * percentage;
	  mySky.SkyEnergy = sunsetSky.SkyEnergy * (1 - percentage) + nightSky.SkyEnergy * percentage;

	  mySky.SunColor = new Color(sunsetSky.SunColor.r * (1 - percentage) + nightSky.SunColor.r * percentage
		   , sunsetSky.SunColor.g * (1 - percentage) + nightSky.SunColor.g * percentage
		   , sunsetSky.SunColor.b * (1 - percentage) + nightSky.SunColor.b * percentage);

	  mySky.SunLatitude = sunsetSky.SunLatitude * (1 - percentage) + nightSky.SunLatitude * percentage;
	  mySky.SunCurve = sunsetSky.SunCurve * (1 - percentage) + nightSky.SunCurve * percentage;
	  mySky.SunAngleMax = sunsetSky.SunAngleMax * (1 - percentage) + nightSky.SunAngleMax * percentage;
	  mySky.SunAngleMin = sunsetSky.SunAngleMin * (1 - percentage) + nightSky.SunAngleMin * percentage;
	  mySky.SunEnergy = sunsetSky.SunEnergy * (1 - percentage) + nightSky.SunEnergy * percentage;

	  countdown = 60;
	  }
  
	  // countdown--;
	  if (countdown == 0)
	  {
		//GetParent().GetNode("Sky_texture").Call("set_sun_position", new Vector3(timePassed*10,timePassed*10, timePassed*10));
		//	countdown = 60;
	  }
	  //Environment.BackgroundEnergy = sunset.BackgroundEnergy * (1 - percentage) + night.BackgroundEnergy * percentage;

	  Environment.AmbientLightColor = new Color(sunset.AmbientLightColor.r * (1 - percentage) + night.AmbientLightColor.r * percentage
		  , sunset.AmbientLightColor.g * (1 - percentage) + night.AmbientLightColor.g * percentage
		  , sunset.AmbientLightColor.b * (1 - percentage) + night.AmbientLightColor.b * percentage);

	  Environment.AmbientLightEnergy = sunset.AmbientLightEnergy * (1 - percentage) + night.AmbientLightEnergy * percentage;
	  //Environment.AmbientLightSkyContribution = sunset.AmbientLightSkyContribution * (1 - percentage) + night.AmbientLightSkyContribution * percentage;
*/
	var environment = GetParent().GetNode("Sky").GetNode("WorldEnvironment") as WorldEnvironment;

	  environment.Environment.FogColor = new Color(sunset.FogColor.r * (1 - percentage) + night.FogColor.r * percentage
		  , sunset.FogColor.g * (1 - percentage) + night.FogColor.g * percentage
		  , sunset.FogColor.b * (1 - percentage) + night.FogColor.b * percentage);

	  environment.Environment.FogSunColor = new Color(sunset.FogSunColor.r * (1 - percentage) + night.FogSunColor.r * percentage
		  , sunset.FogSunColor.g * (1 - percentage) + night.FogSunColor.g * percentage
		  , sunset.FogSunColor.b * (1 - percentage) + night.FogSunColor.b * percentage);

	  environment.Environment.FogSunAmount = 0 ;

	  environment.Environment.FogDepthBegin = sunset.FogDepthBegin * (1 - percentage) + night.FogDepthBegin * percentage;
	  environment.Environment.FogDepthEnd = sunset.FogDepthEnd * (1 - percentage) + night.FogDepthEnd * percentage;
		

			GetParent().GetNode("Sky").Call("set_time_of_day", 0.42-timePassed/(15f*transistionTime));
	}
  }


  public void _on_Sky_texture_sky_updated()
  {
	//GetParent().GetNode("Sky_texture").Call("copy_to_environment", Environment);
  }

}
