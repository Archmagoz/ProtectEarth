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

			// PROPERTIES
			var properties = type.GetProperties(
				BindingFlags.Instance |
				BindingFlags.Public |
				BindingFlags.NonPublic);

			foreach (var prop in properties)
			{
				if (!Attribute.IsDefined(prop, typeof(ExportAttribute)))
					continue;

				if (!typeof(Node).IsAssignableFrom(prop.PropertyType))
					continue;

				var value = prop.GetValue(owner);

				if (value == null)
				{
					GD.PrintErr($"{prop.Name} is not assigned on {type.Name}.");
					hasErrors = true;
				}
			}

			// FIELDS
			var fields = type.GetFields(
				BindingFlags.Instance |
				BindingFlags.Public |
				BindingFlags.NonPublic);

			foreach (var field in fields)
			{
				if (!Attribute.IsDefined(field, typeof(ExportAttribute)))
					continue;

				if (!typeof(Node).IsAssignableFrom(field.FieldType))
					continue;

				var value = field.GetValue(owner);

				if (value == null)
				{
					GD.PrintErr($"{field.Name} is not assigned on {type.Name}.");
					hasErrors = true;
				}
			}

			if (hasErrors) owner.GetTree().Quit();
		}
	}
}