using System;
using System.Reflection;
using System.Collections.Generic;

using Godot;

namespace ProtectEarth.Core.Utils
{
	public static class NodeValidator
	{
		// Cache to avoid repeated reflection per type (performance).
		private static readonly Dictionary<Type, List<MemberInfo>> _cache = new();

		public static void ValidateExports(this Node owner)
		{
#if DEBUG
			var type = owner.GetType();
			bool hasErrors = false;

			// Try to reuse cached members for this type.
			if (!_cache.TryGetValue(type, out var members))
			{
				members = new List<MemberInfo>();

				// Get all instance fields (public + private).
				var fields = type.GetFields(BindingFlags.Instance |
											BindingFlags.Public |
											BindingFlags.NonPublic);

				// Get all instance properties (public + private).
				var properties = type.GetProperties(BindingFlags.Instance |
													BindingFlags.Public |
													BindingFlags.NonPublic);

				// Filter only fields marked with [Export].
				foreach (var field in fields)
					if (Attribute.IsDefined(field, typeof(ExportAttribute)))
						members.Add(field);

				// Filter only properties marked with [Export].
				foreach (var prop in properties)
					if (Attribute.IsDefined(prop, typeof(ExportAttribute)))
						members.Add(prop);

				// Store result in cache for future calls.
				_cache[type] = members;
			}

			// Validate all cached export members.
			foreach (var member in members)
			{
				// Resolve value depending on member type.
				object value = member switch
				{
					FieldInfo f => f.GetValue(owner),
					PropertyInfo p => p.GetValue(owner),
					_ => null
				};

				// If null, it's a missing assignment in the editor.
				if (value == null)
				{
					GD.PrintErr($"{member.Name} is not assigned on {type.Name}");
					hasErrors = true;
				}
			}

			// Stop the game if any critical export is missing.
			if (hasErrors) owner.GetTree().Quit();
#endif
			return;
		}
	}
}