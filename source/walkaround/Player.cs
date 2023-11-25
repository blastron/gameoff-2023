using Godot;
using System;

public partial class Player : Area2D
{
	[Export] private double MaxSpeed = 400;
	[Export] private double Acceleration = 3000;
	[Export] private double Deceleration = 5000;
	private double currentVelocity = 0;
	
	[Export] private float interactOffset = 50;

	private AnimatedSprite2D Sprite => _sprite ?? throw new ArgumentNullException(nameof(_sprite));
	[Export] private AnimatedSprite2D? _sprite;

	private CollisionShape2D Collision => _collision ?? throw new ArgumentNullException(nameof(_collision));
	[Export] private CollisionShape2D? _collision;

	public int Facing => Sprite.FlipH ? -1 : 1;

	public double VelocityPercentage => currentVelocity / MaxSpeed;

	public float InteractPosition => GlobalPosition.X + (interactOffset * Facing);

	private Walkaround? parentWalkaround;

	public Vector2 ScreenSize; // ???
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		// Sprite.Play();
		
		parentWalkaround = this.GetTypedParent<Walkaround>();
		if (parentWalkaround == null)
		{
			GD.PushError("Unable to find a parent walkaround.");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double deltaSeconds)
	{
		if (parentWalkaround == null || !parentWalkaround.IsActive())
		{
			// Freeze all processing if we're not in walkaround mode.
			return;
		}
		
		double inputDirection = (Input.IsActionPressed("move_left") ? -1 : 0) +
								(Input.IsActionPressed("move_right") ? 1 : 0);
		
		if (inputDirection == 0)
		{
			// Decelerate to a stop.
			double frameDeceleration = Deceleration * deltaSeconds;
			currentVelocity = Math.Max(Math.Abs(currentVelocity) - frameDeceleration, 0) * Math.Sign(currentVelocity);
		}
		else if (inputDirection * currentVelocity < 0)
		{
			// We want to change directions. Use the deceleration value to come to a stop before accelerating.
			double frameDeceleration = Deceleration * deltaSeconds;
			currentVelocity = Math.Max(Math.Abs(currentVelocity) - frameDeceleration, 0) * Math.Sign(currentVelocity);
		}
		else
		{
			// Accelerate to the target velocity.
			double frameAcceleration = Acceleration * deltaSeconds;
			currentVelocity = Math.Min(Math.Abs(currentVelocity) + frameAcceleration, MaxSpeed) *
			                  Math.Sign(inputDirection);
		}
		
		double xPos = Position.X;
		xPos += currentVelocity * deltaSeconds;

		// Clamp to the sides of the screen.
		if (GetWalkingBounds(out double leftBounds, out double rightBounds))
		{
			xPos = Math.Clamp(xPos, leftBounds, rightBounds);
		}
		
		Position = new Vector2((float)xPos, Position.Y);


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

	private bool GetWalkingBounds(out double left, out double right)
	{
		if (parentWalkaround == null)
		{
			left = 0;
			right = 0;
			return false;
		}

		parentWalkaround.GetWalkingBounds(out left, out right);

		double boxRelativeCenter = Collision.Position.X;
		if (Sprite.FlipH)
		{
			boxRelativeCenter *= -1;
		}
		
		double boxHalfWidth = Collision.Shape.GetRect().Size.X / 2;
		left -= boxRelativeCenter - boxHalfWidth;
		right -= boxRelativeCenter + boxHalfWidth;

		return true;
	}
}
