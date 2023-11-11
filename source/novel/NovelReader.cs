using System;
using System.Collections.Generic;
using Godot;
using GodotInk;
using Ink.Parsed;

namespace Gameoff2023.source.novel;

public partial class NovelReader : Control
{
	[Export] private InkStory TEMP_Story;

	[Export] private Theme elementTheme;

	private InkStory story => TEMP_Story;

	private VBoxContainer StoryContainer => GetNode<VBoxContainer>("StoryContainer");

	public NovelReader() : base()
	{
		FocusMode = FocusModeEnum.All;
	}

	public override void _Ready()
	{
		story.BindExternalFunction("clear_screen", ClearScreen);
		
		ClearScreen();
		Advance();
	}

	public override void _GuiInput(InputEvent inputEvent)
	{
		bool wasClick = inputEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed;
		
		if ( wasClick || inputEvent.IsActionPressed("ui_select") || inputEvent.IsActionPressed("ui_accept"))
		{
			Control firstFocusable = GetFirstFocusableChild();
			if (firstFocusable is NovelTextBlock textBlock)
			{
				textBlock.Advance(wasClick);
			}
		}
		else if (inputEvent.IsActionPressed("ui_focus_next") || inputEvent.IsActionPressed("ui_focus_prev") ||
				 inputEvent.IsActionPressed("ui_left") || inputEvent.IsActionPressed("ui_right") ||
				 inputEvent.IsActionPressed("ui_up") || inputEvent.IsActionPressed("ui_down"))
		{
			Control firstFocusable = GetFirstFocusableChild();
			firstFocusable?.GrabFocus();
			AcceptEvent();
		}
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
				Label endOfLine = new() { Text = "End of story reached.", Theme = elementTheme };
				StoryContainer.AddChild(endOfLine);
			}
		}
	}

	private void AddLine(string text)
	{
		NovelTextBlock content = new() { Text = text, Theme = elementTheme };
		StoryContainer.AddChild(content);
		
		content.Advanced += OnTextBlockAdvanced;
		content.GrabFocus();
	}

	private void OnTextBlockAdvanced(bool bWasClick)
	{
		if (story.CanContinue)
		{
			Advance();
		}
		else if (!bWasClick)
		{
			// If the story can't continue, it's because we're either waiting for the user to select a choice or we're
			//   at the end of the story. Attempt to pick the first focusable item on the list.
			// We only want to do this if this event didn't come from a mouse click, so as to avoid highlighting an
			//   option unexpectedly.
			Control firstFocusable = GetFirstFocusableChild();
			firstFocusable?.GrabFocus();
		}
	}

	private void AddChoices(IEnumerable<InkChoice> choices)
	{
		// Add a spacer to push the choices to the bottom of the screen.
		StoryContainer.AddSpacer(false);
		
		// List all choices as individual buttons.
		foreach (var choice in choices)
		{
			Button choiceButton = new() { Text = choice.Text, Theme = elementTheme };
			choiceButton.Pressed += delegate { HandleChoice(choice.Index); };

			StoryContainer.AddChild(choiceButton);
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
		foreach (var child in StoryContainer.GetChildren()) child.QueueFree();
	}

	private Control GetFirstFocusableChild()
	{
		foreach (var node in StoryContainer.GetChildren())
		{
			Control child = (Control)node;
			if (child?.FocusMode == FocusModeEnum.All)
			{
				return child;
			}
		}

		return null;
	}
}
