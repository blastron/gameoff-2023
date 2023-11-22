using System;
using Godot.Collections;
using Godot;

public partial class DialogueReader : NovelReader
{
	private Control SpeakerNameWrapper => _speakerNameWrapper ?? throw new ArgumentNullException(nameof(_speakerNameWrapper));
	[Export] private Control? _speakerNameWrapper;

	private RichTextLabel SpeakerName => _speakerName ?? throw new ArgumentNullException(nameof(_speakerName));
	[Export] private RichTextLabel? _speakerName;

	private TextureRect LeftSpeakerTexture => _leftSpeakerTexture ?? throw new ArgumentNullException(nameof(_leftSpeakerTexture));
	[Export] private TextureRect? _leftSpeakerTexture;

	private TextureRect RightSpeakerTexture => _rightSpeakerTexture ?? throw new ArgumentNullException(nameof(_rightSpeakerTexture));
	[Export] private TextureRect? _rightSpeakerTexture;

	[Export] private Array<CharacterData> characters = new();

	[Export] private Texture2D? unknownLeftSprite;
	[Export] private Texture2D? unknownRightSprite;

	public override void Reset()
	{
		base.Reset();
		
		SpeakerNameWrapper.Hide();
		LeftSpeakerTexture.Hide();
		RightSpeakerTexture.Hide();
	}

	public override void SetSpeakerName(string name)
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

	public override void SetLeftCharacter(string name, string expression)
	{
		SetCharacter(name, expression, LeftSpeakerTexture, true);
	}

	public override void SetRightCharacter(string name, string expression)
	{
		SetCharacter(name, expression, RightSpeakerTexture, false);
	}

	private void SetCharacter(string name, string expression, TextureRect texture, bool isLeftSide)
	{
		if (name == "")
		{
			texture.Hide();
			return;
		}
		
		texture.Show();
		foreach (CharacterData character in characters)
		{
			if (!character.name.Equals(name, StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}

			// If this character's sprites were authored for the other side of the screen, flip them to the correct one.
			texture.FlipH = isLeftSide != character.isLeftSprite;
			
			// Look through the list of sprites configured on the character to find one that matches the given
			//   expression.
			foreach (CharacterSpriteData sprite in character.sprites)
			{
				if (!sprite.expression.Equals(expression, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				texture.Texture = sprite.sprite;
				return;
			}
			
			// Couldn't find the expression. Show the fallback sprite.
			GD.PushError("Couldn't find expression " + expression + " for character " + name + ".");
			texture.Texture = character.unknownSprite;
			return;
		}
		
		// Didn't find a matching character.
		GD.PushError("Couldn't find character data for " + name + ".");
		texture.FlipH = false;
		texture.Texture = isLeftSide ? unknownLeftSprite : unknownRightSprite;
	}
}
