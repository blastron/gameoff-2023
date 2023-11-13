using Godot;
using GodotInk;
using System;

public partial class TestInkIntegration : VBoxContainer
{
	[Export]
	private InkStory? _story;
	private InkStory Story => _story ?? throw new ArgumentNullException(nameof(_story));
	
	public override void _Ready()
	{
		ContinueStory();
	}

	private void ContinueStory()
	{
		foreach (Node child in GetChildren())
			child.QueueFree();

		Label content = new() { Text = Story.ContinueMaximally() };
		AddChild(content);

		foreach (InkChoice choice in Story.CurrentChoices)
		{
			Button button = new() { Text = choice.Text };
			button.Pressed += delegate
			{
				Story.ChooseChoiceIndex(choice.Index);
				ContinueStory();
			};
			AddChild(button);
		}
	}
}
