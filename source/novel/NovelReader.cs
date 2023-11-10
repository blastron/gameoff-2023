using System;
using System.Collections.Generic;
using Godot;
using GodotInk;

namespace Gameoff2023.source.novel;

public partial class NovelReader : VBoxContainer
{
	[Export] private InkStory TEMP_Story;

	private InkStory story => TEMP_Story;

	public override void _Ready()
	{
		story.BindExternalFunction("clear_screen", ClearScreen);
		
		Advance();
	}

	private void Advance()
	{
		if (!story.CanContinue)
		{
			throw new InvalidOperationException("Attempted to advance when the story could not continue.");
		}

		// Display the next line.
		string nextLine = story.Continue();
		AddLine(nextLine);

		if (!story.CanContinue)
		{
			// If we have choices, show them.
			if (story.CurrentChoices.Count > 0)
			{
				// Check for special choice flows.
				
				
				AddChoices(story.CurrentChoices);
			}
			else
			{
				// The story can't continue, but we don't have choices. Assume that this is the end of the story.
				AddLine("End of story reached.");
			}
		}
		else
		{
			// TEMP: add a button to manually advance the story
			Button TEMP_advanceButton = new() { Text = "continue..." };
			TEMP_advanceButton.Pressed += delegate
			{
				TEMP_advanceButton.QueueFree();
				Advance();
			};
			
			AddChild(TEMP_advanceButton);
		}
	}

	private void AddLine(string text)
	{
		Label content = new() { Text = text };
		AddChild(content);
	}

	private void AddChoices(IEnumerable<InkChoice> choices)
	{
		foreach (var choice in choices)
		{
			Button choiceButton = new() { Text = choice.Text };
			choiceButton.Pressed += delegate { HandleChoice(choice.Index); };

			AddChild(choiceButton);
		}
	}

	private void HandleChoice(int choiceIndex)
	{
		TEMP_Story.ChooseChoiceIndex(choiceIndex);
		ClearScreen();
		Advance();
	}

	private void ClearScreen()
	{
		// Mark all children for deletion at end of frame.
		foreach (var child in GetChildren()) child.QueueFree();
	}
}
