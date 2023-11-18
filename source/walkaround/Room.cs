using Godot;
using System;

public partial class Room : Node2D
{
	private Sprite2D Background => _background ?? throw new ArgumentNullException(nameof(_background));
	[Export] private Sprite2D? _background;
	
	public float minWalkingBound => 0;
	public float maxWalkingBound => Background.GetRect().Size.X;

	public float minBackgroundBound => 0;
	public float maxBackgroundBound => Background.GetRect().Size.X;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
