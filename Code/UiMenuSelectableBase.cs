using Godot;
using System;

public abstract class UiMenuSelectableBase : UiIndexedObject
{
	public enum State {Default = 0, Highlighted = 1, Pressed = 2};
	
	public abstract void SetState(State state);
}
