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
				Label endOfLine = new() { Text = "End of story reached." };
				AddChild(endOfLine);
			}
		}
	}

	private void AddLine(string text)
	{
		NovelTextBlock content = new() { Text = text };
		AddChild(content);
		
		content.Advanced += OnTextBlockAdvanced;
		content.GrabFocus();
	}

	private void OnTextBlockAdvanced()
	{
		if (story.CanContinue)
		{
			Advance();
		}
		else
		{
			// If the story can't continue, it's because we're either waiting for the user to select a choice or we're
			//   at the end of the story. Attempt to pick the first focusable item on the list.
			foreach (var node in GetChildren())
			{
				Control child = (Control)node;
				if (child.FocusMode == FocusModeEnum.All)
				{
					child.GrabFocus();
					break;
				}
			}
		}
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
