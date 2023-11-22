using Godot;

[GlobalClass]
public partial class CharacterSpriteData : Resource
{
	[Export] public string expression = "";
	[Export] public Texture2D? sprite;
}