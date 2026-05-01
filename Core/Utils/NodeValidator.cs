using Godot;

namespace ProtectEarth.Core.Utils
{
	public static class NodeValidator
	{
		// Reports all null exports at once; returns false if any failed.
		public static bool Require<T>(T node, string name, string owner) where T : class
		{
			if (node != null) return true;
			GD.PushError($"{name} is not assigned on {owner}.");
			return false;
		}
	}
}