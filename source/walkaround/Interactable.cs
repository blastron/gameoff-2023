using Godot;
using System;

public partial class Interactable : Area2D
{
	private AnimatedSprite2D Sprite => _sprite ?? throw new ArgumentNullException(nameof(_sprite));
	[Export] private AnimatedSprite2D? _sprite;

	private CollisionShape2D Collision => _collision ?? throw new ArgumentNullException(nameof(_collision));
	[Export] private CollisionShape2D? _collision;

	[Export] public string Tag = "";
	
	public event Action? Interacted;
	
	public bool Highlighted
	{
		get => _highlighted;
		set
		{
			_highlighted = value;

			if (Sprite.Material is ShaderMaterial shader)
			{
				shader.SetShaderParameter("enabled", _highlighted);
			}
		}
	}
	private bool _highlighted;

	public void Interact(Player player)
	{
		Interacted?.Invoke();
	}

	// Tests to see if the given position is within the horizontal bounds of the defined collision.
	public bool PositionWithinBounds(float xPos)
	{
		float boxCenter = Collision.GlobalPosition.X;
		float boxHalfWidth = Collision.Shape.GetRect().Size.X / 2;
		
		float boxLeft = boxCenter - boxHalfWidth;
		float boxRight = boxCenter + boxHalfWidth;

		return xPos >= boxLeft && xPos <= boxRight;
	}
}
