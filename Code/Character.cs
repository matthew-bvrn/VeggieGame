using Godot;
using System;

public class Character : KinematicBody
{
	public enum State {Active, Shop};
	
	[Export] public float speed; //normal 9f
	public bool collision = false;
	public int frame = 0;
	Vector3 translation;
	public int cameraIndex = 0;
	public State state = State.Active;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
			if(state == State.Active)
			{
				UpdateCamera();
			}
  }

public void UpdateCamera()
{
	if(Input.IsActionJustPressed("camera_swap"))
	{
		cameraIndex++;
		cameraIndex=cameraIndex%2;
		foreach(Node child in GetNode("Follower").GetChildren())
		{
			if(child.Name.Contains("Camera"))
			{
				if(((CameraUser)child).index == cameraIndex)
				{
					((CameraUser)child).Current = true;
				}
				else
				{
					((CameraUser)child).Current = false;
				}
			}
		}
	}
}

public override void _PhysicsProcess(float delta)
	{
		if(state == State.Active)
		{
			translation = new Vector3(0,0,0);

			if (Input.IsActionPressed("move_right"))
			{
					// Move as long as the key/button is pressed.
					translation.x -= 1;
			}
			if (Input.IsActionPressed("move_left"))
			{
					// Move as long as the key/button is pressed.
					translation.x += 1;
			}
			if (Input.IsActionPressed("move_up"))
			{
					// Move as long as the key/button is pressed.
					translation.z += 1;
			}
			if (Input.IsActionPressed("move_down"))
			{
					// Move as long as the key/button is pressed.
					translation.z -= 1;
			}

			translation = translation.Normalized() * speed * delta;

			if(!TestMove(Transform, translation))
			{
				MoveAndCollide(translation);
			}
			
			Vector3 temp = ((Spatial)GetNode("Follower")).Translation+((Spatial)GetNode("Follower").GetNode("Sprite3D")).Translation;
			((Spatial)GetNode("CollisionShape")).Translation = new Vector3(0,temp.y,0);
		}
	}
}



