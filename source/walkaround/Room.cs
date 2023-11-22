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
	
	public event Action<string>? ChoiceInteracted;

	public override void _Ready()
	{
		foreach (Node child in GetChildren())
		{
			if (child is Interactable interactable)
			{
				interactable.Interacted += () => OnInteract(interactable);
			}
		}
	}

	private void OnInteract(Interactable interactable)
	{
		ChoiceInteracted?.Invoke(interactable.Tag);
	}
}

