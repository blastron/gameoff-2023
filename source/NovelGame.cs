using System;
using Godot;
using GodotInk;

public partial class NovelGame : Node2D
{
	[Export] private InkStory? _story;
	[Export] private NarrativeReader? _narrativeReader;
	[Export] private DialogueReader? _dialogueReader;
	[Export] private Walkaround? _walkaround;
	
	private InkStory Story => _story ?? throw new ArgumentNullException(nameof(_story));

	private bool TEMP_AlreadyShownEndOfStory;


	public enum NarrativeMode
	{
		Narration,
		Dialogue,
		Walkaround
	}

	public NarrativeMode currentMode { get; private set; } = NarrativeMode.Narration;

	private NovelReader? ActiveReader
	{
		get
		{
			return currentMode switch
			{
				NarrativeMode.Narration => _narrativeReader ?? throw new ArgumentNullException(nameof(_narrativeReader)),
				NarrativeMode.Dialogue => _dialogueReader ?? throw new ArgumentNullException(nameof(_dialogueReader)),
				_ => null
			};
		}
	}

	public override void _Ready()
	{
		// TODO: Validate configuration and quit if things are missing
		if (_narrativeReader != null) BindReaderEvents(_narrativeReader);
		if (_dialogueReader != null) BindReaderEvents(_dialogueReader);
		if (_walkaround != null) BindWalkaroundEvents(_walkaround);
		
		Story.BindExternalFunction("clear_screen", ClearScreen);
		Story.BindExternalFunction("narration_mode", () => SetMode(NarrativeMode.Narration));
		Story.BindExternalFunction("dialogue_mode", () => SetMode(NarrativeMode.Dialogue));
		Story.BindExternalFunction("walkaround_mode", (string location, string spawn) =>
		{
			SetMode(NarrativeMode.Walkaround);
			ChangeLocation(location, spawn);
		});
		Story.BindExternalFunction("speaker_name", (string name) => SetSpeakerName(name));
		Story.BindExternalFunction("left_character", (string name, string expression) => SetLeftCharacter(name, expression));
		Story.BindExternalFunction("right_character", (string name, string expression) => SetRightCharacter(name, expression));
		
		PrepareReader();
		Advance();
	}

	public bool HasTaggedChoice(string tag)
	{
		foreach (InkChoice choice in Story.CurrentChoices)
		{
			if (choice.Tags != null && choice.Tags.Contains(tag))
			{
				return true;
			}
		}

		return false;
	}

	private void BindReaderEvents(NovelReader reader)
	{
		reader.TypingCompleted += OnTypingCompleted;
		reader.TextAdvanced += OnTextAdvanced;
		reader.ChoiceSelected += OnChoiceSelectedByIndex;
	}

	private void BindWalkaroundEvents(Walkaround walkaround)
	{
		walkaround.ChoiceInteracted += OnChoiceSelectedByTag;
	}

	private void OnTypingCompleted()
	{
		if (ActiveReader != null && Story.CurrentChoices.Count > 0)
		{
			ActiveReader.AddChoices(Story.CurrentChoices);
		}
	}

	private void OnTextAdvanced()
	{
		Advance();
	}

	private void OnChoiceSelectedByIndex(int choiceIndex)
	{
		if (choiceIndex < 0 || choiceIndex >= Story.CurrentChoices.Count)
		{
			throw new ArgumentOutOfRangeException(nameof(choiceIndex));
		}
		
		Story.ChooseChoiceIndex(choiceIndex);
		ClearScreen();
		Advance();
	}

	private void OnChoiceSelectedByTag(string choiceTag)
	{
		for (int choiceIndex = 0; choiceIndex < Story.CurrentChoices.Count; choiceIndex++)
		{
			InkChoice choice = Story.CurrentChoices[choiceIndex];
			if (choice.Tags != null && choice.Tags.Contains(choiceTag))
			{
				Story.ChooseChoiceIndex(choiceIndex);
				Advance();
				return;
			}
		}
		
		GD.PushError("Attempted to select choice with tag \"" + choiceTag + "\" but no matching choice was found.");
	}

	private void Advance()
	{
		if (!Story.CanContinue && AtEndOfStory())
		{
			OnEndOfStory();
			return;
		}

		// Continue the story, then if we have an active reader showing, show the next line. Note that the call to
		//   Story.Continue() here may change modes, changing (or nulling) ActiveReader in the process.
		string nextLine = Story.Continue();
		ActiveReader?.AddLine(nextLine);
	}

	private void SetMode(NarrativeMode newMode)
	{
		if (newMode != currentMode)
		{
			currentMode = newMode;
			switch (newMode)
			{
				case NarrativeMode.Narration:
				case NarrativeMode.Dialogue:
					PrepareReader();
					break;
				case NarrativeMode.Walkaround:
					PrepareWalkaround();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(newMode), newMode, null);
			}
		}
	}
	
	private void SetSpeakerName(string name)
	{
		if (ActiveReader == null) GD.PushError("Attempted to set speaker name while no reader was active.");
		ActiveReader?.SetSpeakerName(name);
	}

	private void SetLeftCharacter(string name, string expression)
	{
		if (ActiveReader == null) GD.PushError("Attempted to set visible character while no reader was active.");
		ActiveReader?.SetLeftCharacter(name, expression);
	}

	private void SetRightCharacter(string name, string expression)
	{
		if (ActiveReader == null) GD.PushError("Attempted to set visible character while no reader was active.");
		ActiveReader?.SetRightCharacter(name, expression);
	}

	// Shows the correct reader for the current narrative mode and resets it to a blank state.
	private void PrepareReader()
	{
		// Switch to the correct reader.
		switch (currentMode)
		{
			case NarrativeMode.Narration:
				_dialogueReader?.Hide();
				_narrativeReader?.Show();
				break;
			case NarrativeMode.Dialogue:
				_narrativeReader?.Hide();
				_dialogueReader?.Show();
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(currentMode), currentMode,
					"Reader is not available in non-text modes.");
		}
		
		_walkaround?.Hide();
		
		ActiveReader?.Reset();
		ActiveReader?.GrabFocus();
	}

	private void PrepareWalkaround()
	{
		_walkaround?.Show();

		_dialogueReader?.Hide();
		_narrativeReader?.Hide();
	}

	private void ChangeLocation(string location, string spawn)
	{
		_walkaround?.LoadRoom(location);
	}

	private void ClearScreen()
	{
		ActiveReader?.ClearScreen();
	}

	private bool AtEndOfStory()
	{
		return !Story.CanContinue && Story.CurrentChoices.Count == 0;
	}

	private void OnEndOfStory()
	{
		if (TEMP_AlreadyShownEndOfStory)
		{
			GD.PrintErr("Already called EOS.");
			return;
		}

		TEMP_AlreadyShownEndOfStory = true;
		switch (currentMode)
		{
			case NarrativeMode.Narration:
			case NarrativeMode.Dialogue:
				ActiveReader?.AddRawLabel("End of story reached.");
				break;
			default:
				SetMode(NarrativeMode.Narration);
				ActiveReader?.AddRawLabel("End of story reached.");
				break;
		}
	}
}
