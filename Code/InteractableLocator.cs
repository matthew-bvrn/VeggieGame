using Godot;
using System;

public class InteractableLocator : Node
{
	Spatial m_parent;
	VisualInstance m_selectionBox;
	float m_distance;
	bool m_hasMultipleOptions;
	
  public InteractableLocator(Spatial parent, VisualInstance selectionBox, float distance, bool hasMultipleOptions) 
	{
		m_parent=parent;
		m_distance=distance;
		m_selectionBox=selectionBox;
		m_hasMultipleOptions = hasMultipleOptions;
	}
	
	public Node GetNearestNode()
	{
		Vector3 characterTranslation = ((Spatial)m_parent.GetTree().CurrentScene.GetNode("Character")).GlobalTransform.origin;
		float distanceToChar =  characterTranslation.DistanceTo(m_parent.GlobalTransform.origin);
		if(distanceToChar<m_distance)
		{
			if(m_selectionBox.Visible == false)
			{
				m_selectionBox.Visible = true;
			}

			if (m_hasMultipleOptions)
			{
				Node closestSpot = ((Node)m_parent.GetChildren()[0]);
				float closestDistance = 1000000;
				foreach (Node node in m_parent.GetChildren())
				{
					if(node != m_selectionBox)
					{
					float distance = (((Spatial)node).GlobalTransform.origin).DistanceTo(characterTranslation);
					if (distance < closestDistance)
					{
						closestDistance = distance;
						closestSpot = node;
					}}
				}
				m_selectionBox.Translation = ((Spatial)closestSpot).Translation;
				return closestSpot;
			}
			else
			{
				return m_parent;
      }
		}
		else
		{
			if(m_selectionBox.Visible == true)
			{
				m_selectionBox.Visible = false;
			}
			return null;
		}
	}
}
