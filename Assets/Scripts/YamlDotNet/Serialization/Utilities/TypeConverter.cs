using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace YamlDotNet.Serialization.Utilities
{
	public static class TypeConverter
	{
		public delegate bool TryParseDelegate<T>(string value, out T result);

		public static void RegisterTypeConverter<TConvertible, TConverter>() where TConverter : System.ComponentModel.TypeConverter
		{
			if (!TypeDescriptor.GetAttributes(typeof(TConvertible)).OfType<TypeConverterAttribute>().Any((TypeConverterAttribute a) => a.ConverterTypeName == typeof(TConverter).AssemblyQualifiedName))
			{
				TypeDescriptor.AddAttributes(typeof(TConvertible), new TypeConverterAttribute(typeof(TConverter)));
			}
		}

		public static T ChangeType<T>(object value)
		{
			return (T)ChangeType(value, typeof(T));
		}

		public static T ChangeType<T>(object value, IFormatProvider provider)
		{
			return (T)ChangeType(value, typeof(T), provider);
		}

		public static T ChangeType<T>(object value, CultureInfo culture)
		{
			return (T)ChangeType(value, typeof(T), culture);
		}

		public static object ChangeType(object value, Type destinationType)
		{
			return ChangeType(value, destinationType, CultureInfo.InvariantCulture);
		}

		public static object ChangeType(object value, Type destinationType, IFormatProvider provider)
		{
			return ChangeType(value, destinationType, new CultureInfoAdapter(CultureInfo.CurrentCulture, provider));
		}

		public static object ChangeType(object value, Type destinationType, CultureInfo culture)
		{
			if (value == null || value is DBNull)
			{
				return (!destinationType.IsValueType()) ? null : Activator.CreateInstance(destinationType);
			}
			Type type = value.GetType();
			if (destinationType.IsAssignableFrom(type))
			{
				return value;
			}
			if (destinationType.IsGenericType())
			{
				Type genericTypeDefinition = destinationType.GetGenericTypeDefinition();
				if (genericTypeDefinition == typeof(Nullable<>))
				{
					Type destinationType2 = destinationType.GetGenericArguments()[0];
					object obj = ChangeType(value, destinationType2, culture);
					return Activator.CreateInstance(destinationType, obj);
				}
			}
			if (destinationType.IsEnum())
			{
				string text = value as string;
				return (text == null) ? value : Enum.Parse(destinationType, text, true);
			}
			if (destinationType == typeof(bool))
			{
				if ("0".Equals(value))
				{
					return false;
				}
				if ("1".Equals(value))
				{
					return true;
				}
			}
			System.ComponentModel.TypeConverter converter = TypeDescriptor.GetConverter(value);
			if (converter != null && converter.CanConvertTo(destinationType))
			{
				return converter.ConvertTo(null, culture, value, destinationType);
			}
			System.ComponentModel.TypeConverter converter2 = TypeDescriptor.GetConverter(destinationType);
			if (converter2 != null && converter2.CanConvertFrom(type))
			{
				return converter2.ConvertFrom(null, culture, value);
			}
			Type[] array = new Type[2] { type, destinationType };
			foreach (Type type2 in array)
			{
				foreach (MethodInfo publicMethod in type2.GetPublicMethods())
				{
					if (!publicMethod.IsSpecialName || (!(publicMethod.Name == "op_Implicit") && !(publicMethod.Name == "op_Explicit")) || !destinationType.IsAssignableFrom(publicMethod.ReturnParameter.ParameterType))
					{
						continue;
					}
					ParameterInfo[] parameters = publicMethod.GetParameters();
					if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(type))
					{
						try
						{
							return publicMethod.Invoke(null, new object[1] { value });
						}
						catch (TargetInvocationException ex)
						{
							throw ex.Unwrap();
						}
					}
				}
			}
			if (type == typeof(string))
			{
				try
				{
					MethodInfo publicStaticMethod = destinationType.GetPublicStaticMethod("Parse", typeof(string), typeof(IFormatProvider));
					if (publicStaticMethod != null)
					{
						return publicStaticMethod.Invoke(null, new object[2] { value, culture });
					}
					publicStaticMethod = destinationType.GetPublicStaticMethod("Parse", typeof(string));
					if (publicStaticMethod != null)
					{
						return publicStaticMethod.Invoke(null, new object[1] { value });
					}
				}
				catch (TargetInvocationException ex2)
				{
					throw ex2.Unwrap();
				}
			}
			if (destinationType == typeof(TimeSpan))
			{
				return TimeSpan.Parse((string)ChangeType(value, typeof(string), CultureInfo.InvariantCulture));
			}
			return Convert.ChangeType(value, destinationType, CultureInfo.InvariantCulture);
		}

		public static T TryParse<T>(string value) where T : struct
		{
			switch (typeof(T).GetTypeCode())
			{
			case TypeCode.Boolean:
				return (T)(object)TryParse<bool>(value, bool.TryParse);
			case TypeCode.Byte:
				return (T)(object)TryParse<byte>(value, byte.TryParse);
			case TypeCode.DateTime:
				return (T)(object)TryParse<DateTime>(value, DateTime.TryParse);
			case TypeCode.Decimal:
				return (T)(object)TryParse<decimal>(value, decimal.TryParse);
			case TypeCode.Double:
				return (T)(object)TryParse<double>(value, double.TryParse);
			case TypeCode.Int16:
				return (T)(object)TryParse<short>(value, short.TryParse);
			case TypeCode.Int32:
				return (T)(object)TryParse<int>(value, int.TryParse);
			case TypeCode.Int64:
				return (T)(object)TryParse<long>(value, long.TryParse);
			case TypeCode.SByte:
				return (T)(object)TryParse<sbyte>(value, sbyte.TryParse);
			case TypeCode.Single:
				return (T)(object)TryParse<float>(value, float.TryParse);
			case TypeCode.UInt16:
				return (T)(object)TryParse<ushort>(value, ushort.TryParse);
			case TypeCode.UInt32:
				return (T)(object)TryParse<uint>(value, uint.TryParse);
			case TypeCode.UInt64:
				return (T)(object)TryParse<ulong>(value, ulong.TryParse);
			default:
				throw new NotSupportedException(string.Format("Cannot parse type '{0}'.", typeof(T).FullName));
			}
		}

		public static T? TryParse<T>(string value, TryParseDelegate<T> parse) where T : struct
		{
			T result;
			return (!parse(value, out result)) ? null : new T?(result);
		}
	}
}
