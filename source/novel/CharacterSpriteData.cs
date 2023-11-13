using Godot;

namespace Gameoff2023.source.novel;

[GlobalClass]
public partial class CharacterSpriteData : Resource
{
	[Export] public string expression = "";
	[Export] public Texture2D? sprite;
}