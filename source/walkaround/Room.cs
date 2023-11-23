using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Room : Node2D
{
	[Export] private Sprite2D? Background;

	private List<SpawnPoint> spawnPoints = new();
	
	public float minWalkingBound => 0;
	public float maxWalkingBound => Background?.GetRect().Size.X ?? 0;

	public float minBackgroundBound => 0;
	public float maxBackgroundBound => Background?.GetRect().Size.X ?? 0;
	
	public event Action<string>? ChoiceInteracted;

	public override void _Ready()
	{
		foreach (Interactable interactable in this.GetTypedChildren<Interactable>())
		{
			interactable.Interacted += () => OnInteract(interactable);
		}

		spawnPoints = this.GetTypedChildren<SpawnPoint>().ToList();
	}

	public float? GetSpawnLocation(string name)
	{
		foreach (SpawnPoint spawnPoint in spawnPoints)
		{
			if (spawnPoint.Name == name)
			{
				return spawnPoint.GlobalPosition.X;
			}
		}

		return null;
	}

	private void OnInteract(Interactable interactable)
	{
		ChoiceInteracted?.Invoke(interactable.Tag);
	}
}

