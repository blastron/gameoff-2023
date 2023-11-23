using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;

public partial class Interactable : Area2D
{
	[Export] public string Tag = "";

	private List<AnimatedSprite2D> animatedSprites = new();
	private List<Sprite2D> staticSprites = new();
	private List<CollisionShape2D> collisions = new();
	
	public event Action? Interacted;
	
	public bool Highlighted
	{
		get => _highlighted;
		set
		{
			_highlighted = value;
			
			foreach (AnimatedSprite2D sprite in animatedSprites)
			{
				if (sprite.Material is ShaderMaterial shader)
				{
					shader.SetShaderParameter("enabled", _highlighted);
				}
			}

			foreach (Sprite2D sprite in staticSprites)
			{
				if (sprite.Material is ShaderMaterial shader)
				{
					shader.SetShaderParameter("enabled", _highlighted);
				}
			}
		}
	}
	private bool _highlighted;

	public override void _Ready()
	{
		animatedSprites = this.GetTypedChildren<AnimatedSprite2D>().ToList();
		staticSprites = this.GetTypedChildren<Sprite2D>().ToList();
		collisions = this.GetTypedChildren<CollisionShape2D>().ToList();
	}

	public void Interact(Player player)
	{
		Interacted?.Invoke();
	}

	// Tests to see if the given position is within the horizontal bounds of the defined collision.
	public bool PositionWithinBounds(float xPos)
	{
		float? minLeft = null;
		float? maxRight = null;
		
		foreach (CollisionShape2D collisionShape in collisions)
		{
			float boxCenter = collisionShape.GlobalPosition.X;
			float boxHalfWidth = collisionShape.Shape.GetRect().Size.X / 2;
		
			float boxLeft = boxCenter - boxHalfWidth;
			float boxRight = boxCenter + boxHalfWidth;

			minLeft = Math.Min(boxLeft, minLeft ?? boxLeft);
			maxRight = Math.Max(boxRight, maxRight ?? boxRight);
		}
		
		return xPos >= minLeft && xPos <= maxRight;
	}
}
