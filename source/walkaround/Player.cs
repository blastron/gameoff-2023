using Godot;
using System;

public partial class Player : Area2D
{
	[Export] private double MaxSpeed { get; set; } = 400;
	
	private AnimatedSprite2D Sprite => _sprite ?? throw new ArgumentNullException(nameof(_sprite));
	[Export] private AnimatedSprite2D? _sprite;

	private CollisionShape2D Collision => _collision ?? throw new ArgumentNullException(nameof(_collision));
	[Export] private CollisionShape2D? _collision;

	private Walkaround? parentWalkaround;

	public Vector2 ScreenSize; // ???

	private double velocity = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		// Sprite.Play();

		Node? parent = GetParent();
		while (parent != null)
		{
			if (parent is Walkaround castParent)
			{
				parentWalkaround = castParent;
				break;
			}

			parent = parent.GetParent();
		}

		if (parentWalkaround == null)
		{
			GD.PushError("Unable to find a parent walkaround.");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double deltaSeconds)
	{
		double inputDirection = (Input.IsActionPressed("move_left") ? -1 : 0) +
								(Input.IsActionPressed("move_right") ? 1 : 0);
		
		float xPos = Position.X;
		
		// todo: acceleration
		velocity = inputDirection * MaxSpeed;
		xPos += (float)(velocity * deltaSeconds);

		// Clamp to the sides of the screen.
		if (GetWalkingBounds(out var leftBounds, out var rightBounds))
		{
			xPos = Math.Clamp(xPos, leftBounds, rightBounds);
		}
		
		Position = new Vector2(xPos, Position.Y);


		if (inputDirection != 0)
		{
			// Flip sprite facing to match input direction.
			Sprite.FlipH = inputDirection < 0;
			
			// Sprite.Animation = "flying";
		}
		else
		{
			// Sprite.Animation = "hovering";
		}
	}

	private bool GetWalkingBounds(out float left, out float right)
	{
		if (parentWalkaround == null)
		{
			left = 0;
			right = 0;
			return false;
		}

		parentWalkaround.GetWalkingBounds(out left, out right);

		float boxWidth = Collision.Shape.GetRect().Size.X;
		left += boxWidth / 2;
		right -= boxWidth / 2;

		return true;
	}
}