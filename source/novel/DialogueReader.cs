using System;
using Godot;
using GodotInk;

namespace Gameoff2023.source.novel;

public partial class DialogueReader : NovelReader
{
	[Export] private Control? _speakerNameWrapper;
	private Control SpeakerNameWrapper => _speakerNameWrapper ?? throw new ArgumentNullException(nameof(_speakerNameWrapper));

	[Export] private RichTextLabel? _speakerName;
	private RichTextLabel SpeakerName => _speakerName ?? throw new ArgumentNullException(nameof(_speakerName));


	public override void Reset()
	{
		base.Reset();
		
		SpeakerNameWrapper.Hide();
	}
	
	public override void SetSpeaker(string name)
	{
		if (name == SpeakerName.Text)
		{
			return;
		}
		
		ClearScreen();
		
		if (name.Length > 0)
		{
			SpeakerNameWrapper.Show();
			SpeakerName.Text = name;
		}
		else
		{
			SpeakerNameWrapper.Hide();
		}
	}
}
