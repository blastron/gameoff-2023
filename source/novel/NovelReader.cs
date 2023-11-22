using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotInk;

public partial class NovelReader : Control
{
	[Export] private Theme? elementTheme;

	[Export] private VBoxContainer? _textContainer;
	private VBoxContainer TextContainer => _textContainer ?? throw new ArgumentNullException(nameof(_textContainer));
	
	[Export] private VBoxContainer? _choiceContainer;
	private VBoxContainer ChoiceContainer => _choiceContainer ?? throw new ArgumentNullException(nameof(_choiceContainer));
	
	// The parent game. Could be null if we're running this scene independently of a full game for testing purposes.
	protected NovelGame? parentGame;

	public NovelReader() : base()
	{
		FocusMode = FocusModeEnum.All;
	}
	
	public event Action? TypingCompleted;
	public event Action? TextAdvanced;
	public event Action<int>? ChoiceSelected;

	public override void _Ready()
	{
		base._Ready();
		
		parentGame = this.GetTypedParent<NovelGame>();
	}

	public override void _GuiInput(InputEvent inputEvent)
	{
		bool wasClick = inputEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed;
		
		if ( wasClick || inputEvent.IsActionPressed("ui_select") || inputEvent.IsActionPressed("ui_accept"))
		{
			if (ChoiceContainer.GetChildren().Count > 0)
			{
				// We have option buttons, but they've lost focus. Select the first one.
				Button? firstOption = ChoiceContainer.GetChildren().First() as Button;
				firstOption?.GrabFocus();
			}
			else if (TextContainer.GetChildren().Count > 0)
			{
				NovelTextBlock? lastLine = TextContainer.GetChildren().Last() as NovelTextBlock;
				if (lastLine != null && lastLine.IsTyping())
				{
					lastLine.SkipTyping();
				}
				else
				{
					TextAdvanced?.Invoke();
				}
			}
			else
			{
				// We somehow have no children. As a failsafe, advance the story.
				TextAdvanced?.Invoke();
			}
		}
		else if (inputEvent.IsActionPressed("ui_up") || inputEvent.IsActionPressed("ui_down"))
		{
			// We have focus. Check to see if there's an option that we can focus, and if so, focus it. Either way,
			//   don't allow navigation away from this control.
			if (ChoiceContainer.GetChildren().Count > 0)
			{
				Button? option = (inputEvent.IsActionPressed("ui_up")
					? ChoiceContainer.GetChildren().Last()
					: ChoiceContainer.GetChildren().First()) as Button;
				option?.GrabFocus();
			}
			
			AcceptEvent();
		}
		else if (inputEvent.IsActionPressed("ui_focus_next") || inputEvent.IsActionPressed("ui_focus_prev") ||
				 inputEvent.IsActionPressed("ui_left") || inputEvent.IsActionPressed("ui_right"))
		{
			// We have focus. Don't allow navigation away from this control.
			// TODO: We'll probably want to bubble up ui_focus_prev in order to show an options menu.
			AcceptEvent();
		}
	}

	public virtual void Reset()
	{
		ClearScreen();
	}

	public virtual void SetSpeakerName(string name)
	{
		GD.PushError("Attempted to set speaker name in an unsupported context.");
	}

	public virtual void SetLeftCharacter(string name, string expression)
	{
		GD.PushError("Attempted to set left character sprite in an unsupported context.");
	}

	public virtual void SetRightCharacter(string name, string expression)
	{
		GD.PushError("Attempted to set right character sprite in an unsupported context.");
	}

	public void AddLine(string text)
	{
		NovelTextBlock textBlock = new() { Text = text, Theme = elementTheme };
		TextContainer.AddChild(textBlock);

		textBlock.TypingCompleted += () => TypingCompleted?.Invoke();
	}

	public void AddRawLabel(string text)
	{
		Label endOfLine = new() { Text = text, Theme = elementTheme };
		TextContainer.AddChild(endOfLine);
	}

	public void AddChoices(IEnumerable<InkChoice> choices)
	{
		// List all choices as individual buttons.
		foreach (var choice in choices)
		{
			Button choiceButton = new() { Text = choice.Text, Theme = elementTheme };
			choiceButton.Pressed += () => ChoiceSelected?.Invoke(choice.Index);

			ChoiceContainer.AddChild(choiceButton);
		}
	}

	public void ClearScreen()
	{
		// Mark all children for deletion at end of frame.
		foreach (var child in TextContainer.GetChildren()) child.QueueFree();
		foreach (var child in ChoiceContainer.GetChildren()) child.QueueFree();
		
		GrabFocus();
	}
}
