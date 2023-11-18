using Godot;
using System;

public partial class Walkaround : Node2D
{
	private Camera2D Camera => _camera ?? throw new ArgumentNullException(nameof(_camera));
	[Export] private Camera2D? _camera;

	private Player Player => _player ?? throw new ArgumentNullException(nameof(_player));
	[Export] private Player? _player;

	private Room Room => _tempRoom ?? throw new ArgumentNullException(nameof(_tempRoom));
	[Export] private Room? _tempRoom;

	private float screenWidth;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		screenWidth = GetViewportRect().Size.X;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// TEMP: center camera on player center
		float cameraPosition = Player.Position.X - screenWidth / 2;
		
		// Clamp camera bounds within the screen bounds.
		GetCameraBounds(out float leftCameraBounds, out float rightCameraBounds);
		cameraPosition = Math.Clamp(cameraPosition, leftCameraBounds, rightCameraBounds);
		
		Camera.Position = new Vector2(cameraPosition, 0);
	}

	public void GetWalkingBounds(out float left, out float right)
	{
		left = Room.minWalkingBound;
		right = Room.maxWalkingBound;
	}

	private void GetCameraBounds(out float left, out float right)
	{
		left = Room.minBackgroundBound;
		right = Room.maxBackgroundBound - screenWidth;
	}
}
