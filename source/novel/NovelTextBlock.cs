using System;
using Godot;

namespace Gameoff2023.source.novel;

// A text block that shows novel text and handles the event.
public partial class NovelTextBlock : RichTextLabel
{
	private double timeToNextCharacter;
	
	public NovelTextBlock() : base()
	{
		FocusMode = FocusModeEnum.None;
		
		AutowrapMode = TextServer.AutowrapMode.WordSmart;
		FitContent = true;
		ScrollActive = false;
		BbcodeEnabled = true;

		VisibleCharacters = 0;
	}

	// Fired when the text reaches the end of its scroll.
	public event Action? TypingCompleted;

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
					TypingCompleted?.Invoke();
				}
			}
		}
	}

	public bool IsTyping()
	{
		return VisibleCharacters < GetTotalCharacterCount();
	}

	public void SkipTyping()
	{
		if (IsTyping())
		{
			VisibleCharacters = GetTotalCharacterCount();
			TypingCompleted?.Invoke();
		}
	}
}