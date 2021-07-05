using Godot;
using System;
using System.Diagnostics;

public class UiMenuHighlight : UiMenuSelectableBase
{
	public override void SetState(State state)
	{
		switch(state)
		{
			case State.Default:
				Visible = false;
				break;
			case State.Highlighted:
				Visible = true;
				break;
			case State.Pressed:
				break;
		}
	}
}
