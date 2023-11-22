using Godot;
using System;
using System.Diagnostics;
using System.Linq;

public partial class Walkaround : Node2D
{

	private Player Player => _player ?? throw new ArgumentNullException(nameof(_player));
	[Export] private Player? _player;

	private Room Room => _tempRoom ?? throw new ArgumentNullException(nameof(_tempRoom));
	[Export] private Room? _tempRoom;

	// The parent game. Could be null if we're running this scene independently of a full game for testing purposes.
	private NovelGame? parentGame;
	
	private Camera2D? camera;

	private float screenWidth;

	private Interactable? interactTarget;

	public event Action<string>? ChoiceInteracted;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		
		screenWidth = GetViewportRect().Size.X;

		parentGame = this.GetTypedParent<NovelGame>();
		camera = parentGame?.GetTypedChildren<Camera2D>().FirstOrDefault();
		
		// TODO: set this up when room is changed
		Room.ChoiceInteracted += OnChoiceInteracted;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!IsActive()) return;
		
		ProcessCamera(delta);
		SelectInteractable();
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (IsActive())
		{
			if (inputEvent.IsActionPressed("interact") && interactTarget != null)
			{
				GetViewport().SetInputAsHandled();
				interactTarget.Interact(Player);
			}
		}
		else
		{
			base._Input(inputEvent);
		}
	}

	public bool IsActive()
	{
		return parentGame == null || parentGame.currentMode == NovelGame.NarrativeMode.Walkaround;
	}

	private void OnChoiceInteracted(string choiceTag)
	{
		ChoiceInteracted?.Invoke(choiceTag);
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

	private void ProcessCamera(double delta)
	{
		if (camera == null)
		{
			return;
		}
		
		// TEMP: center camera on player center
		float cameraPosition = Player.Position.X - screenWidth / 2;
		
		// Clamp camera bounds within the screen bounds.
		GetCameraBounds(out float leftCameraBounds, out float rightCameraBounds);
		cameraPosition = Math.Clamp(cameraPosition, leftCameraBounds, rightCameraBounds);
		
		camera.Position = new Vector2(cameraPosition, 0);
	}

	private void SelectInteractable()
	{
		// TEMP: use player's center to determine location.
		float interactPosition = Player.Position.X;

		Interactable? newTarget = null;
		foreach (Node child in Room.GetChildren())
		{
			if (child is Interactable interactable && interactable.PositionWithinBounds(interactPosition))
			{
				newTarget = interactable;
				break;
			}
		}

		if (newTarget != interactTarget)
		{
			if (newTarget != null)
			{
				newTarget.Highlighted = true;
			}

			if (interactTarget != null)
			{
				interactTarget.Highlighted = false;
			}
			
			interactTarget = newTarget;
		}
	}
}
