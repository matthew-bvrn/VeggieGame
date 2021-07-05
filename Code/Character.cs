using Godot;
using System;

public class Character : KinematicBody
{
  public enum State { Active, Shop };

  [Export] public float m_speed; //normal 9f
  public bool m_collision = false;
  public int m_frame = 0;
  Vector3 m_translation;
  public int m_cameraIndex = 0;
  public State m_state = State.Active;

  public override void _Process(float delta)
  {
	if (m_state == State.Active)
	{
	  UpdateCamera();
	}
  }

  public void UpdateCamera()
  {
	if (Input.IsActionJustPressed("camera_swap"))
	{
	  m_cameraIndex++;
	  m_cameraIndex = m_cameraIndex % 2;
	  foreach (Node child in GetNode("Follower").GetChildren())
	  {
		if (child.Name.Contains("Camera"))
		{
		  if (((CameraUser)child).index == m_cameraIndex)
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
	if (m_state == State.Active)
	{
	  m_translation = new Vector3(0, 0, 0);

	  if (Input.IsActionPressed("move_right"))
	  {
		// Move as long as the key/button is pressed.
		m_translation.x -= 1;
	  }
	  if (Input.IsActionPressed("move_left"))
	  {
		// Move as long as the key/button is pressed.
		m_translation.x += 1;
	  }
	  if (Input.IsActionPressed("move_up"))
	  {
		// Move as long as the key/button is pressed.
		m_translation.z += 1;
	  }
	  if (Input.IsActionPressed("move_down"))
	  {
		// Move as long as the key/button is pressed.
		m_translation.z -= 1;
	  }

	  m_translation = m_translation.Normalized() * m_speed * delta;

	  if (!TestMove(Transform, m_translation))
	  {
		MoveAndCollide(m_translation);
	  }
		
	}
  }
}



