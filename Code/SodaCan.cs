using Godot;
using System;

public class SodaCan : RigidBody
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

private void _on_Soda_can_body_entered(object body)
{
	GD.Print("hit");
}
private void _on_Soda_can_body_shape_entered(int body_id, object body, int body_shape, int local_shape)
{
   GD.Print("hit");
}


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}




