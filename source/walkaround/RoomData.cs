using Godot;

[GlobalClass]
public partial class RoomData : Resource
{
	[Export] public string Name = "";
	[Export] public PackedScene? Scene;
}