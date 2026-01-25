using System;
using System.Collections.Generic;
using System.Reflection;

namespace YamlDotNet
{
	internal static class ReflectionExtensions
	{
		private static readonly FieldInfo remoteStackTraceField = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);

		public static bool IsValueType(this Type type)
		{
			return type.IsValueType;
		}

		public static bool IsGenericType(this Type type)
		{
			return type.IsGenericType;
		}

		public static bool IsInterface(this Type type)
		{
			return type.IsInterface;
		}

		public static bool IsEnum(this Type type)
		{
			return type.IsEnum;
		}

		public static bool HasDefaultConstructor(this Type type)
		{
			return type.IsValueType || type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null) != null;
		}

		public static TypeCode GetTypeCode(this Type type)
		{
			return Type.GetTypeCode(type);
		}

		public static IEnumerable<PropertyInfo> GetPublicProperties(this Type type)
		{
			return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
		}

		public static IEnumerable<MethodInfo> GetPublicMethods(this Type type)
		{
			return type.GetMethods(BindingFlags.Static | BindingFlags.Public);
		}

		public static MethodInfo GetPublicStaticMethod(this Type type, string name, params Type[] parameterTypes)
		{
			return type.GetMethod(name, BindingFlags.Static | BindingFlags.Public, null, parameterTypes, null);
		}

		public static Exception Unwrap(this TargetInvocationException ex)
		{
			Exception innerException = ex.InnerException;
			if (remoteStackTraceField != null)
			{
				remoteStackTraceField.SetValue(ex.InnerException, ex.InnerException.StackTrace + "\r\n");
			}
			return innerException;
		}
	}
}
