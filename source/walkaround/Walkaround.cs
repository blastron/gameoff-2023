using Godot;
using System;
using System.Linq;
using Godot.Collections;

public partial class Walkaround : Node2D
{
	private Player Player => _player ?? throw new ArgumentNullException(nameof(_player));
	[Export] private Player? _player;

	// The amount of space on either side of the stage where the camera will remain stationary.
	[Export] private double cameraPlayerGutter = 300;

	[Export] private double cameraDeadZoneHalfWidth = 50;
	private double cameraDeadZonePosition = 0;

	[Export] private Node2D? RoomContainer;

	[Export] private Array<RoomData> rooms = new();
	
	private Room? currentRoom;
	public string currentRoomName { get; private set; }

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
		if (roomName == currentRoomName)
		{
			return;
		}
		
		if (rooms.Count == 0)
		{
			GD.PushError("No rooms were configured.");
			return;
		}

		for (int roomIndex = 0; roomIndex < rooms.Count; roomIndex++)
		{
			if (rooms[roomIndex].Name.Equals(roomName, StringComparison.OrdinalIgnoreCase))
			{
				GD.Print("Loading room \"" + roomName + "\".");
				currentRoomName = roomName;
				LoadRoom(roomIndex);
				return;
			}
		}

		// Couldn't find the room. As a fallback, load room 0.
		GD.PushError("Unable to find room with ID \"" + roomName + "\". Falling back to room 0.");
		currentRoomName = rooms[0].Name;
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

	public void WarpToPoint(string pointName)
	{
		float? pointPos = currentRoom?.GetSpawnLocation(pointName);
		Player.Position = new Vector2(pointPos ?? Player.Position.X, Player.Position.Y);

		cameraDeadZonePosition = Player.Position.X;

		if (pointPos == null)
		{
			GD.PushError("Unable to find spawn point with ID \"" + pointName + "\". Not moving.");
		}
		else
		{
			GD.Print("Jumping to spawn point \"" + pointName + "\".");
		}
	}

	private void OnChoiceInteracted(string choiceTag)
	{
		ChoiceInteracted?.Invoke(choiceTag);
	}

	public void GetWalkingBounds(out double left, out double right)
	{
		left = currentRoom?.minWalkingBound ?? 0;
		right = currentRoom?.maxWalkingBound ?? 0;
	}

	private void GetCameraBounds(out double left, out double right)
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
		
		// We want to position the camera such that the player's horizontal position on the screen matches their
		//   relative position on the stage itself, so that when the character is physically near the left side of the
		//   room, they will be near the left side of the screen.
		GetWalkingBounds(out double leftWalkingBounds, out double rightWalkingBounds);
		GetCameraBounds(out double leftCameraBounds, out double rightCameraBounds);

		// Adjust the walking bounds to account for the gutter on either side of the screen where we don't want the
		//   camera to move.
		// TODO: handle cases where the walking bounds are significantly smaller than the edge of the stage
		leftWalkingBounds += cameraPlayerGutter;
		rightWalkingBounds -= cameraPlayerGutter;
		
		// In order to minimize the amount of camera motion when the player is making small movements, track a dead zone
		//   around the player that gets pushed around when the player moves.
		double playerPosition = Player.Position.X;
		if (Math.Abs(cameraDeadZonePosition - playerPosition) > cameraDeadZoneHalfWidth)
		{
			// TODO: Add some sort of easing when the dead zone starts to move, then have the dead zone recenter a bit
			//   when the player stops.
			cameraDeadZonePosition = playerPosition +
			                         cameraDeadZoneHalfWidth * Math.Sign(cameraDeadZonePosition - playerPosition);
		}
		
		// Get the percentage along the horizontal movement track of the dead zone for conversion to camera coordinates.
		double positionPercentage = leftWalkingBounds < rightWalkingBounds
			? (cameraDeadZonePosition - leftWalkingBounds) / (rightWalkingBounds - leftWalkingBounds)
			: 0.5f;
		positionPercentage = Math.Clamp(positionPercentage, 0, 1);
		
		// Ease in/out to make the edges smoother.
		const double exponent = 1.5;
		positionPercentage = Math.Pow(positionPercentage, exponent) /
		                     (Math.Pow(positionPercentage, exponent) + Math.Pow(1 - positionPercentage, exponent));

		// Convert to camera coordinates.
		double cameraPosition = (rightCameraBounds - leftCameraBounds) * positionPercentage + leftCameraBounds;
		camera.Position = new Vector2((float)cameraPosition, 0);
	}

	private void SelectInteractable()
	{
		// TEMP: use player's center to determine location.
		float interactPosition = Player.InteractPosition;

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
