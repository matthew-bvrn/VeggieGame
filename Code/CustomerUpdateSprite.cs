using Godot;
using System;

public class CustomerUpdateSprite : TextureRect
{
	public float oscillationInterval;
	public float oscillationWidth;
	public float neutralHorizontalLocation;
	public float upwardsSpeed;
	public float lifetime;
	public float age = 0;
	public float phaseOffset;


	public override void _Process(float delta)
	{
		age += delta;
		Modulate = new Color(Modulate.r, Modulate.g, Modulate.b, 1-age/lifetime);
		SetPosition(new Vector2(neutralHorizontalLocation + oscillationWidth * ((float)Math.Sin((age+phaseOffset) / oscillationInterval)), GetViewport().GetVisibleRect().Size.y - age * upwardsSpeed));
		//SetPosition(new Vector2(1600,0));
		if(age>lifetime)
		{
			QueueFree();
		}
	}
}
