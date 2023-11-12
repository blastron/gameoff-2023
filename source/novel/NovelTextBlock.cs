using System;
using Godot;

namespace Gameoff2023.source.novel;

// A text block that shows novel text and handles the event.
public partial class NovelTextBlock : Label
{
	private double timeToNextCharacter;
	
	public NovelTextBlock() : base()
	{
		FocusMode = FocusModeEnum.All;
		AutowrapMode = TextServer.AutowrapMode.WordSmart;

		VisibleCharacters = 0;
	}

	// Fired when the text reaches the end of its scroll. Parameter indicates if the user skipped the scroll.
	public event Action<bool> Completed;
	
	// Fired when the user has advanced to the next block. Parameter indicates if the event was triggered via
	//   mouse click.
	public event Action<bool> Advanced;

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (VisibleCharacters < GetTotalCharacterCount())
		{
			timeToNextCharacter -= delta;
			if (timeToNextCharacter <= 0)
			{
				VisibleCharacters += 1;
				timeToNextCharacter += 0.02;
				
				if (VisibleCharacters == GetTotalCharacterCount())
				{
					Completed?.Invoke(false);
				}
			}
		}
	}

	public override void _GuiInput(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed("ui_select") || inputEvent.IsActionPressed("ui_accept"))
		{
			AcceptEvent();
			Advance(false);
		}
		else if (inputEvent.IsActionPressed("ui_focus_next") || inputEvent.IsActionPressed("ui_focus_prev") ||
			inputEvent.IsActionPressed("ui_left") || inputEvent.IsActionPressed("ui_right") ||
			inputEvent.IsActionPressed("ui_up") || inputEvent.IsActionPressed("ui_down"))
		{
			// Don't allow navigation away from this element.
			AcceptEvent();
		}
	}

	public void Advance(bool fromClick)
	{
		if (VisibleCharacters < GetTotalCharacterCount())
		{
			VisibleCharacters = GetTotalCharacterCount();
			Completed?.Invoke(true);
		}
		else
		{
			// A block that's been advanced past is no longer focusable.
			FocusMode = FocusModeEnum.None;
			Advanced?.Invoke(fromClick);
		}
	}
}