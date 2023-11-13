using System;
using Gameoff2023.source.novel;
using Godot;
using GodotInk;

namespace Gameoff2023.source;

public partial class NovelGame : Node2D
{
	[Export] private InkStory? _story;
	[Export] private NarrativeReader? _narrativeReader;
	[Export] private DialogueReader? _dialogueReader;
	
	private InkStory Story => _story ?? throw new ArgumentNullException(nameof(_story));

	private bool TEMP_AlreadyShownEndOfStory;


	private enum NarrativeMode
	{
		Narration,
		Dialogue
	}

	private NarrativeMode currentMode = NarrativeMode.Narration;

	private NovelReader ActiveReader
	{
		get
		{
			return currentMode switch
			{
				NarrativeMode.Narration => _narrativeReader ?? throw new ArgumentNullException(nameof(_narrativeReader)),
				NarrativeMode.Dialogue => _dialogueReader ?? throw new ArgumentNullException(nameof(_dialogueReader)),
				_ => throw new ArgumentOutOfRangeException()
			};
		}
	}

	public override void _Ready()
	{
		// TODO: Validate configuration and quit if things are missing
		if (_narrativeReader != null) BindReaderEvents(_narrativeReader);
		if (_dialogueReader != null) BindReaderEvents(_dialogueReader);
		
		Story.BindExternalFunction("clear_screen", ClearScreen);
		Story.BindExternalFunction("narration_mode", () => SetMode(NarrativeMode.Narration));
		Story.BindExternalFunction("dialogue_mode", () => SetMode(NarrativeMode.Dialogue));
		Story.BindExternalFunction("speaker_name", (string name) => SetSpeakerName(name));
		Story.BindExternalFunction("left_character", (string name, string expression) => SetLeftCharacter(name, expression));
		Story.BindExternalFunction("right_character", (string name, string expression) => SetRightCharacter(name, expression));
		
		PrepareReader();
		Advance();
	}

	private void BindReaderEvents(NovelReader reader)
	{
		reader.TypingCompleted += OnTypingCompleted;
		reader.TextAdvanced += OnTextAdvanced;
		reader.ChoiceSelected += OnOptionSelected;
	}

	private void OnTypingCompleted()
	{
		if (Story.CurrentChoices.Count > 0)
		{
			ActiveReader.AddChoices(Story.CurrentChoices);
		}
	}

	private void OnTextAdvanced()
	{
		Advance();
	}

	private void OnOptionSelected(int choiceIndex)
	{
		Story.ChooseChoiceIndex(choiceIndex);
		ClearScreen();
		Advance();
	}
	
	private void Advance()
	{
		if (!Story.CanContinue && AtEndOfStory())
		{
			OnEndOfStory();
			return;
		}

		string nextLine = Story.Continue();
		ActiveReader.AddLine(nextLine);
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
				default:
					throw new ArgumentOutOfRangeException(nameof(newMode), newMode, null);
			}
		}
	}
	
	private void SetSpeakerName(string name)
	{
		ActiveReader.SetSpeakerName(name);
	}

	private void SetLeftCharacter(string name, string expression)
	{
		ActiveReader.SetLeftCharacter(name, expression);
	}

	private void SetRightCharacter(string name, string expression)
	{
		ActiveReader.SetRightCharacter(name, expression);
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
		
		ActiveReader.Reset();
		ActiveReader.GrabFocus();
	}

	private void ClearScreen()
	{
		ActiveReader.ClearScreen();
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
				ActiveReader.AddRawLabel("End of story reached.");
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(currentMode), currentMode, null);
		}
	}
}
