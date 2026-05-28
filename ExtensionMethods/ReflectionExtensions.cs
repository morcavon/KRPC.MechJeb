using System;
using System.Linq;
using System.Reflection;

namespace KRPC.MechJeb.ExtensionMethods {
	public static class ReflectionExtensions {
		private const BindingFlags DefaultLookupFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;

		public static T CreateInstance<T>(this Type type, object[] args) {
			try {
				Type[] types = Type.EmptyTypes;
				if(args != null) {
					types = new Type[args.Length];
					for(int i = 0; i < args.Length; i++)
						types[i] = args[i].GetType();
				}

				return (T)type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, types, null).Invoke(args);
			}
			catch(Exception ex) {
				Logger.Severe("Coudn't create an instance of " + type, ex);
				throw new MJServiceException(ex.ToString());
			}
		}

		public static FieldInfo GetCheckedField(this Type type, string name) {
			return GetField(type, name, DefaultLookupFlags).CheckIfExists(type, name);
		}

		public static FieldInfo GetCheckedField(this Type type, string name, BindingFlags bindingAttr) {
			return GetField(type, name, bindingAttr).CheckIfExists(type, name);
		}

		public static FieldInfo GetOptionalField(this Type type, string name) {
			return GetField(type, name, DefaultLookupFlags).LogIfExists(type, name);
		}

		public static FieldInfo GetOptionalField(this Type type, string name, BindingFlags bindingAttr) {
			return GetField(type, name, bindingAttr).LogIfExists(type, name);
		}

		public static PropertyInfo GetCheckedProperty(this Type type, string name) {
			return GetProperty(type, name, DefaultLookupFlags).CheckIfExists(type, name);
		}

		public static PropertyInfo GetOptionalProperty(this Type type, string name) {
			return GetProperty(type, name, DefaultLookupFlags).LogIfExists(type, name);
		}

		public static MethodInfo GetCheckedMethod(this Type type, string name) {
			return GetMethod(type, name, DefaultLookupFlags).CheckIfExists(type, name + "()");
		}

		public static MethodInfo GetCheckedMethod(this Type type, string name, Type[] types) {
			return GetMethod(type, name, DefaultLookupFlags, types).CheckIfExists(type, name + "()");
		}

		public static MethodInfo GetCheckedMethod(this Type type, string name, BindingFlags bindingAttr) {
			return GetMethod(type, name, bindingAttr).CheckIfExists(type, name + "()");
		}

		public static MethodInfo GetOptionalMethod(this Type type, string name) {
			return GetMethod(type, name, DefaultLookupFlags).LogIfExists(type, name + "()");
		}

		public static MethodInfo GetOptionalMethod(this Type type, string name, BindingFlags bindingAttr) {
			return GetMethod(type, name, bindingAttr).LogIfExists(type, name + "()");
		}

		private static FieldInfo GetField(Type type, string name, BindingFlags bindingAttr) {
			FieldInfo exact = type.GetField(name, bindingAttr);
			if(exact != null)
				return exact;

			FieldInfo[] fields = type.GetFields(bindingAttr);
			FieldInfo ignoredCase = fields.FirstOrDefault(field => string.Equals(field.Name, name, StringComparison.OrdinalIgnoreCase));
			if(ignoredCase != null)
				return ignoredCase;

			string normalized = NormalizeMemberName(name);
			return fields.FirstOrDefault(field => NormalizeMemberName(field.Name) == normalized);
		}

		private static PropertyInfo GetProperty(Type type, string name, BindingFlags bindingAttr) {
			PropertyInfo exact = null;
			try {
				exact = type.GetProperty(name, bindingAttr);
			}
			catch(AmbiguousMatchException) { }
			if(exact != null)
				return exact;

			PropertyInfo[] properties = type.GetProperties(bindingAttr);
			PropertyInfo ignoredCase = properties.FirstOrDefault(property => string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase));
			if(ignoredCase != null)
				return ignoredCase;

			string normalized = NormalizeMemberName(name);
			return properties.FirstOrDefault(property => NormalizeMemberName(property.Name) == normalized);
		}

		private static MethodInfo GetMethod(Type type, string name, BindingFlags bindingAttr) {
			MethodInfo exact = null;
			try {
				exact = type.GetMethod(name, bindingAttr);
			}
			catch(AmbiguousMatchException) { }
			if(exact != null)
				return exact;

			MethodInfo[] methods = type.GetMethods(bindingAttr);
			MethodInfo ignoredCase = methods.FirstOrDefault(method => string.Equals(method.Name, name, StringComparison.OrdinalIgnoreCase));
			if(ignoredCase != null)
				return ignoredCase;

			string normalized = NormalizeMemberName(name);
			return methods.FirstOrDefault(method => NormalizeMemberName(method.Name) == normalized);
		}

		private static MethodInfo GetMethod(Type type, string name, BindingFlags bindingAttr, Type[] types) {
			MethodInfo exact = null;
			try {
				exact = type.GetMethod(name, bindingAttr, null, types, null);
			}
			catch(AmbiguousMatchException) { }
			if(exact != null)
				return exact;

			return type.GetMethods(bindingAttr)
				.Where(method => MemberNameMatches(method.Name, name))
				.FirstOrDefault(method => method.GetParameters().Select(parameter => parameter.ParameterType).SequenceEqual(types));
		}

		private static bool MemberNameMatches(string actual, string expected) {
			return string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase) || NormalizeMemberName(actual) == NormalizeMemberName(expected);
		}

		private static string NormalizeMemberName(string name) {
			return new string(name.Where(ch => ch != '_').ToArray()).ToLowerInvariant();
		}

		private static T CheckIfExists<T>(this T obj, Type type, string name) {
			if(obj == null) {
				string error = type + "." + name + " not found";
				Logger.Severe(error);
				MechJeb.errors.Add(error);
			}
			else
				Logger.Debug(type + "." + name + " found");

			return obj;
		}

		private static T LogIfExists<T>(this T obj, Type type, string name) {
			if(obj != null)
				Logger.Debug(type + "." + name + " found");

			return obj;
		}

		public static object GetInstanceValue(this FieldInfo field, object instance) {
			return field != null && instance != null ? field.GetValue(instance) : null;
		}
	}
}
