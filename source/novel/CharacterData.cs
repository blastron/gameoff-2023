using Godot;
using Godot.Collections;

[GlobalClass]
public partial class CharacterData : Resource
{
	[Export] public string name;
	[Export] public Array<CharacterSpriteData> sprites;
	[Export] public Texture2D? unknownSprite;
	[Export] public bool isLeftSprite = false;

	CharacterData()
	{
		name = "";
		sprites = new Array<CharacterSpriteData>();
	}
}