using Godot;

public static class NodeExtensions
{
	/// <summary>
	/// Sets the parent of the node, handling both orphaned nodes and existing children.
	/// Optionally keeps the global transform (position/rotation/scale).
	/// </summary>
	public static bool SetParent(this Node node, Node newParent, bool keepGlobalTransform = true)
	{
		if (node.GetParent() == newParent) return false;

		if (node.GetParent() != null)
			node.Reparent(newParent, keepGlobalTransform);
		else
			newParent.AddChild(node);
		
		
		return true;
	}
}