using System;
using System.Reflection;
using Godot;

namespace ProtectEarth.Core.Utils
{
	public static class NodeValidator
	{
		public static void ValidateExports(this Node owner)
		{
			var type = owner.GetType();
			bool hasErrors = false;

			// Fetch ALL members (fields, properties, methods, etc).
			// We're filtering later, so this is intentionally broad.
			var members = type.GetMembers(
				BindingFlags.Instance |
				BindingFlags.Public |
				BindingFlags.NonPublic);

			foreach (var member in members)
			{
				// Only care about members explicitly marked with [Export].
				if (!Attribute.IsDefined(member, typeof(ExportAttribute)))
					continue;

				// Resolve runtime value depending on member type.
				// Anything that is not a field/property will fall through as null.
				object value = member switch
				{
					PropertyInfo prop => prop.GetValue(owner),
					FieldInfo field => field.GetValue(owner),
					_ => null
				};

				// If null, it's either:
				// - not assigned in inspector
				// - or not a field/property (false positive from GetMembers)
				if (value == null)
				{
					// Simple, readable error — no extra noise.
					GD.PrintErr($"{member.Name} is not assigned on {type.Name}");
					hasErrors = true;
				}
			}

			// Hard fail-fast: stops execution immediately if anything is missing.
			// Good for catching setup issues early, but kills the whole tree.
			if (hasErrors) owner.GetTree().Quit();
		}
	}
}