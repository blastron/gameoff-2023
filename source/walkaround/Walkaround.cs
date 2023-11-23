using Godot;
using System;
using System.Linq;
using Godot.Collections;

public partial class Walkaround : Node2D
{
	private Player Player => _player ?? throw new ArgumentNullException(nameof(_player));
	[Export] private Player? _player;

	[Export] private Node2D? RoomContainer;

	[Export] private Array<RoomData> rooms = new();

	private Room? currentRoom;

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
		
		// Delete any Rooms in the scene, which should have only been in there as placeholders.
		if (RoomContainer != null)
		{
			foreach (Node childNode in RoomContainer.GetChildren())
			{
				childNode.QueueFree();
			}
		}
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

	public void LoadRoom(string roomName)
	{
		if (rooms.Count == 0)
		{
			GD.PushError("No rooms were configured.");
			return;
		}

		for (int roomIndex = 0; roomIndex < rooms.Count; roomIndex++)
		{
			if (rooms[roomIndex].Name.Equals(roomName, StringComparison.OrdinalIgnoreCase))
			{
				GD.Print("Loading room \"" + roomName + "\" at index " + roomIndex + ".");
				LoadRoom(roomIndex);
				return;
			}
		}

		// Couldn't find the room. As a fallback, load room 0.
		GD.PushError("Unable to find room with ID \"" + roomName + "\". Falling back to room 0.");
		LoadRoom(0);
	}

	private void LoadRoom(int roomIndex)
	{
		currentRoom?.QueueFree();

		currentRoom = rooms[roomIndex].Scene?.Instantiate<Room>();
		if (currentRoom != null)
		{
			(RoomContainer ?? this).AddChild(currentRoom);
			currentRoom.ChoiceInteracted += OnChoiceInteracted;
		}
	}

	private void OnChoiceInteracted(string choiceTag)
	{
		ChoiceInteracted?.Invoke(choiceTag);
	}

	public void GetWalkingBounds(out float left, out float right)
	{
		left = currentRoom?.minWalkingBound ?? 0;
		right = currentRoom?.maxWalkingBound ?? 0;
	}

	private void GetCameraBounds(out float left, out float right)
	{
		left = currentRoom?.minBackgroundBound ?? 0;
		right = currentRoom?.maxBackgroundBound - screenWidth ?? 0;
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
		if (currentRoom != null)
		{
			foreach (Interactable child in currentRoom.GetTypedChildren<Interactable>())
			{
				bool hasMatchingChoice = parentGame?.HasTaggedChoice(child.Tag) ?? true;
				if (hasMatchingChoice && child.PositionWithinBounds(interactPosition))
				{
					newTarget = child;
					break;
				}
			}
		}

		if (newTarget != interactTarget)
		{
			if (newTarget != null)
			{
				newTarget.Highlighted = true;
			}

			if (interactTarget != null && IsInstanceValid(interactTarget))
			{
				interactTarget.Highlighted = false;
			}
			
			interactTarget = newTarget;
		}
	}
}
