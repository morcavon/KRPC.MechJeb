using System;
using System.Collections.Generic;
using System.Reflection;

using KRPC.MechJeb.ExtensionMethods;

namespace KRPC.MechJeb {
	public static class EditableDouble {
		internal const string MechJebType = "MuMech.EditableDoubleMult";

		// Fields and methods
		private static PropertyInfo value;
		private static readonly Dictionary<Type, PropertyInfo> valueProperties = new Dictionary<Type, PropertyInfo>();

		internal static void InitType(Type type) {
			value = type.GetCheckedProperty("val");
		}

		public static double Get(object instance) {
			return Convert.ToDouble(GetValueProperty(instance).GetValue(instance, null));
		}

		public static void Set(object instance, double value) {
			GetValueProperty(instance).SetValue(instance, value, null);
		}

		// Helper methods for fields which create a new object every time they are changed in GUI
		public static double Get(FieldInfo field, object instance) {
			return Get(field.GetValue(instance));
		}

		public static void Set(FieldInfo field, object instance, double value) {
			Set(field.GetValue(instance), value);
		}

		private static PropertyInfo GetValueProperty(object instance) {
			if(instance == null)
				throw new MJServiceException(nameof(EditableDouble) + ".Val not found");

			Type type = instance.GetType();
			if(value != null && value.DeclaringType.IsAssignableFrom(type))
				return value;

			if(!valueProperties.TryGetValue(type, out PropertyInfo property)) {
				property = type.GetOptionalProperty("Val") ?? type.GetOptionalProperty("val");
				if(property == null)
					throw new MJServiceException(type + ".Val not found");

				valueProperties.Add(type, property);
			}

			return property;
		}
	}

	public static class EditableInt {
		internal const string MechJebType = "MuMech.EditableInt";

		// Fields and methods
		private static PropertyInfo value;
		private static MemberInfo text;
		private static readonly Dictionary<Type, PropertyInfo> valueProperties = new Dictionary<Type, PropertyInfo>();
		private static readonly Dictionary<Type, MemberInfo> textMembers = new Dictionary<Type, MemberInfo>();

		internal static void InitType(Type type) {
			value = type.GetCheckedProperty("val");
			text = GetOptionalTextMember(type);
		}

		public static int Get(object instance) {
			return Convert.ToInt32(GetValueProperty(instance).GetValue(instance, null));
		}

		public static void Set(object instance, int value) {
			GetValueProperty(instance).SetValue(instance, value, null);
			SetText(instance, value.ToString());
		}

		private static PropertyInfo GetValueProperty(object instance) {
			if(instance == null)
				throw new MJServiceException(nameof(EditableInt) + ".Val not found");

			Type type = instance.GetType();
			if(value != null && value.DeclaringType.IsAssignableFrom(type))
				return value;

			if(!valueProperties.TryGetValue(type, out PropertyInfo property)) {
				property = type.GetOptionalProperty("Val") ?? type.GetOptionalProperty("val");
				if(property == null)
					throw new MJServiceException(type + ".Val not found");

				valueProperties.Add(type, property);
			}

			return property;
		}

		private static MemberInfo GetTextMember(object instance) {
			if(instance == null)
				return null;

			Type type = instance.GetType();
			if(text != null && text.DeclaringType.IsAssignableFrom(type))
				return text;

			if(!textMembers.TryGetValue(type, out MemberInfo member)) {
				member = GetOptionalTextMember(type);
				textMembers.Add(type, member);
			}

			return member;
		}

		private static MemberInfo GetOptionalTextMember(Type type) {
			PropertyInfo property = type.GetOptionalProperty("Text");
			if(property != null)
				return property;

			return type.GetOptionalField("_text") ?? type.GetOptionalField("TextConfig");
		}

		private static void SetText(object instance, string value) {
			MemberInfo member = GetTextMember(instance);
			if(member is PropertyInfo property)
				property.SetValue(instance, value, null);
			else if(member is FieldInfo field)
				field.SetValue(instance, value);
		}
	}

	public static class MovingAverage {
		internal const string MechJebType = "MuMech.MovingAverage";

		// Fields and methods
		private static PropertyInfo value;

		internal static void InitType(Type type) {
			value = type.GetCheckedProperty("value");
		}

		public static double Get(object instance) {
			return (double)value.GetValue(instance, null);
		}
	}

	internal static class EditableAngle {
		private static readonly Dictionary<Type, FieldInfo> degreesFields = new Dictionary<Type, FieldInfo>();
		private static readonly Dictionary<Type, FieldInfo> minutesFields = new Dictionary<Type, FieldInfo>();
		private static readonly Dictionary<Type, FieldInfo> secondsFields = new Dictionary<Type, FieldInfo>();
		private static readonly Dictionary<Type, FieldInfo> negativeFields = new Dictionary<Type, FieldInfo>();

		public static double Get(object instance) {
			if(instance == null)
				throw new MJServiceException(nameof(EditableAngle) + ".targetLongitude not found");

			double value = EditableDouble.Get(GetField(instance, degreesFields, "Degrees"), instance)
				+ EditableDouble.Get(GetField(instance, minutesFields, "Minutes"), instance) / 60.0
				+ EditableDouble.Get(GetField(instance, secondsFields, "Seconds"), instance) / 3600.0;

			return (bool)GetField(instance, negativeFields, "Negative").GetValue(instance) ? -value : value;
		}

		public static void Set(object instance, double value) {
			if(instance == null)
				throw new MJServiceException(nameof(EditableAngle) + ".targetLongitude not found");

			double positiveValue = Math.Abs(value);
			double degrees = Math.Floor(positiveValue);
			double minutesTotal = (positiveValue - degrees) * 60.0;
			double minutes = Math.Floor(minutesTotal);
			double seconds = (minutesTotal - minutes) * 60.0;

			EditableDouble.Set(GetField(instance, degreesFields, "Degrees"), instance, degrees);
			EditableDouble.Set(GetField(instance, minutesFields, "Minutes"), instance, minutes);
			EditableDouble.Set(GetField(instance, secondsFields, "Seconds"), instance, seconds);
			GetField(instance, negativeFields, "Negative").SetValue(instance, value < 0);
		}

		private static FieldInfo GetField(object instance, Dictionary<Type, FieldInfo> cache, string name) {
			Type type = instance.GetType();
			if(!cache.TryGetValue(type, out FieldInfo field)) {
				field = type.GetCheckedField(name);
				cache.Add(type, field);
			}

			return field;
		}
	}
}
