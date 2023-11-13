using Godot;
using GodotInk;

namespace Gameoff2023.source.novel;

public partial class NarrativeReader : NovelReader
{
	public override void SetSpeaker(string name) 
	{
		ClearScreen();
		AddRawLabel("Error: set_speaker called in narrative mode. " + name + " is talking.");
	}
}
