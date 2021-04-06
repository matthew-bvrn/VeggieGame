using Godot;
using System;

[Tool]

public class Cube_Texturelooping : MeshInstance
{
	float scaler = 1.6f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Process(float delta)
	{
		//if(Engine.EditorHint)
		{
			ShaderMaterial mat0 = (ShaderMaterial)GetSurfaceMaterial(0);
			ShaderMaterial mat1 = (ShaderMaterial)GetSurfaceMaterial(1);
			ShaderMaterial mat2 = (ShaderMaterial)GetSurfaceMaterial(2);
			
			float x = ((Scale.x*((Spatial)GetParent()).Scale.x)/scaler);
			float y = ((Scale.z*((Spatial)GetParent()).Scale.z)/scaler);
			
			mat0.SetShaderParam("scalex", x);
			mat0.SetShaderParam("scaley", y);
			
			x = ((Scale.y*((Spatial)GetParent()).Scale.y)/scaler);
			y = ((Scale.z*((Spatial)GetParent()).Scale.z)/scaler);
			
			mat1.SetShaderParam("scalex", x);
			mat1.SetShaderParam("scaley", y);
			
			x = ((Scale.x*((Spatial)GetParent()).Scale.x)/scaler);
			y = ((Scale.y*((Spatial)GetParent()).Scale.y)/scaler);
			
			mat2.SetShaderParam("scalex", y);
			mat2.SetShaderParam("scaley", x);
		}
	}

}
