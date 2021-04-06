using Godot;
using System;

[Tool]

public class Floor_Texturelooping : CSGBox
{
	ShaderMaterial mat;

	// Called when the node enters the scene tree for the first time.
	public override void _Process(float delta)
	{
		if(Engine.EditorHint)
		{
			mat = (ShaderMaterial)Material;
			float x = ((Scale.x*((Spatial)GetParent()).Scale.x)/11.5f);
			float y = ((Scale.z*((Spatial)GetParent()).Scale.z)/11.5f);
			mat.SetShaderParam("scalex", y);
			mat.SetShaderParam("scaley", x);
		}
	}

}
