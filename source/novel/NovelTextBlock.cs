using System;
using Godot;

namespace Gameoff2023.source.novel;

// A text block that shows novel text and handles the event.
public partial class NovelTextBlock : Label
{
	public NovelTextBlock() : base()
	{
		FocusMode = FocusModeEnum.All;
	}

	public event Action Advanced;

	public override void _GuiInput(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed("ui_select") || inputEvent.IsActionPressed("ui_accept"))
		{
			Advance();
		}
		else if (inputEvent.IsActionPressed("ui_focus_next") || inputEvent.IsActionPressed("ui_focus_prev") ||
			inputEvent.IsActionPressed("ui_left") || inputEvent.IsActionPressed("ui_right") ||
			inputEvent.IsActionPressed("ui_up") || inputEvent.IsActionPressed("ui_down"))
		{
			// Don't allow navigation away from this element.
			AcceptEvent();
		}
	}

	public override void _Notification(int notificationCode)
	{
		switch ((long)notificationCode)
		{
			case NotificationFocusEnter:
				Text = ">" + Text;
				break;
			case NotificationFocusExit:
				while (Text.StartsWith(">"))
				{
					Text = Text.Remove(0,1);
				}
				break;
		}
	}

	public void Advance()
	{
		// A block that's been advanced past is no longer focusable.
		FocusMode = FocusModeEnum.None;
		AcceptEvent();
			
		Advanced?.Invoke();
	}
}