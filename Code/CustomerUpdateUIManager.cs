using Godot;
using System;

public class CustomerUpdateUIManager : Node
{
  PackedScene customerUpdateSprite = GD.Load<PackedScene>("res://ObjectsAbstract/CustomerUpdateSprite.tscn");
	RandomNumberGenerator rng = new RandomNumberGenerator();

	public override void _Ready()
  {
		rng.Randomize();
  }

  private void _on_MarketManager_EmitCustomerUpdateSprite(string name, MarketManager.CustomerUpdateType type)
{
	TextureRect node = customerUpdateSprite.Instance() as TextureRect;
	int x = 0;
	switch(type)
	{
	  case MarketManager.CustomerUpdateType.Searching:
		x = 64;
		break;
	  case MarketManager.CustomerUpdateType.Found:
		x = 128;
		break;
	  case MarketManager.CustomerUpdateType.NotFound:
		x = 64*3;
		break;
	}
	int y = ((PlantManager)GetTree().CurrentScene.GetNode("PlantManager")).PlantIndexFromFruitNameLookup(name);
	((AtlasTexture)node.Texture).Region = new Rect2(x, y*64, 64, 64);

		node.Set("upwardsSpeed", rng.RandfRange(50f, 70));
		node.Set("oscillationInterval", rng.RandfRange(1f, 2));
		node.Set("oscillationWidth", rng.RandfRange(60,120));
		node.Set("neutralHorizontalLocation", GetViewport().GetVisibleRect().Size.x - 200 + rng.RandfRange(-40, 40));
		node.Set("lifetime", rng.RandfRange(8, 15));
		node.Set("phaseOffset", rng.RandfRange(0,3.14f));
		AddChild(node);
}

}
