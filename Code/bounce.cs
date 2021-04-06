using Godot;
using System;

public class bounce : Spatial
{
  bool isBounce = false;
  float bouncePercent = 0.0f;
  float bounceTime = 0.0f;
  float bounceLength = 0.2f;
  float bounceHeight = 1.0f;
  float bounceOffset = 0.892f;
  RayCast raycast1 = new RayCast();
  RayCast raycast2 = new RayCast();

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
	raycast1 = GetParent().GetParent().GetNode("RayCast1") as RayCast;
	raycast2 = GetParent().GetParent().GetNode("RayCast2") as RayCast;
  }


  //  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
	if (((Character)GetParent().GetParent()).state == Character.State.Active)
	{
	  raycast1.ForceRaycastUpdate();
	  raycast2.ForceRaycastUpdate();

	  if (Input.IsActionPressed("move_down") || Input.IsActionPressed("move_up") || Input.IsActionPressed("move_left") || Input.IsActionPressed("move_right"))
	  {
		if (isBounce == false)
		{
		  isBounce = true;
		}
	  }

	  if (isBounce == true)
	  {
		Spatial parent = GetParent() as Spatial;
		Vector3 closestCollision = raycast1.GetCollisionPoint();
		Vector3 raycast2collision = raycast2.GetCollisionPoint();
		if (raycast1.GetCollider() != raycast2.GetCollider())
		{
		  if (raycast1.GetCollisionPoint().y < raycast2.GetCollisionPoint().y)
		  {
			closestCollision = raycast2.GetCollisionPoint();
		  }
		}
		parent.Translation = new Vector3(parent.Translation.x, closestCollision.y, parent.Translation.z);

		bounceTime += delta;
		bouncePercent = bounceTime / bounceLength;
		Translation = new Vector3(0, bounceHeight * (-(bouncePercent * bouncePercent) + bouncePercent) + bounceOffset, 0.0f);

		if (bouncePercent >= 1.0f)
		{
		  bouncePercent = 0.0f;
		  bounceTime = 0.0f;
		  isBounce = false;
		  Translation = new Vector3(0, bounceOffset, 0);
		}
	  }
	}
  }
}
