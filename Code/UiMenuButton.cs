using Godot;
using System;

public class UiMenuButton : UiMenuSelectableBase
{
	public override void SetState(State state)
	{
		((AtlasTexture)Texture).Region = new Rect2((int)state*96, ((AtlasTexture)Texture).Region.Position.y, ((AtlasTexture)Texture).Region.Size.x, ((AtlasTexture)Texture).Region.Size.y);
		if(state == State.Pressed)
		{
			ResetState();
	}
	}

	private async void ResetState()
	{
		await ToSignal(GetTree().CreateTimer(0.125f), "timeout");
		SetState(State.Highlighted);
	}
}
