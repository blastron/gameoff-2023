using System.Collections.Generic;
using System.Linq;
using Godot;

public static class GodotExtensions
{
	public static T? GetTypedParent<T>(this Node child) where T : Node
	{
		Node? parent = child.GetParent();
		while (parent != null)
		{
			if (parent is T castParent)
			{
				return castParent;
			}

			parent = parent.GetParent();
		}

		return null;
	}

	public static IEnumerable<T> GetTypedChildren<T>(this Node root) where T : Node
	{
		return root.GetChildren().Where(child => child is T).Cast<T>();
	}
}