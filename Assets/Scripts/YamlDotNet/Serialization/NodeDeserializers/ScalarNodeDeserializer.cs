using System;
using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class ScalarNodeDeserializer : INodeDeserializer
	{
		private static readonly NumberFormatInfo numberFormat = new NumberFormatInfo
		{
			CurrencyDecimalSeparator = ".",
			CurrencyGroupSeparator = "_",
			CurrencyGroupSizes = new int[1] { 3 },
			CurrencySymbol = string.Empty,
			CurrencyDecimalDigits = 99,
			NumberDecimalSeparator = ".",
			NumberGroupSeparator = "_",
			NumberGroupSizes = new int[1] { 3 },
			NumberDecimalDigits = 99
		};

		bool INodeDeserializer.Deserialize(EventReader reader, Type expectedType, Func<EventReader, Type, object> nestedObjectDeserializer, out object value)
		{
			Scalar scalar = reader.Allow<Scalar>();
			if (scalar == null)
			{
				value = null;
				return false;
			}
			if (expectedType.IsEnum())
			{
				value = Enum.Parse(expectedType, scalar.Value);
			}
			else
			{
				switch (expectedType.GetTypeCode())
				{
				case TypeCode.Boolean:
					value = bool.Parse(scalar.Value);
					break;
				case TypeCode.Byte:
					value = byte.Parse(scalar.Value, numberFormat);
					break;
				case TypeCode.Int16:
					value = short.Parse(scalar.Value, numberFormat);
					break;
				case TypeCode.Int32:
					value = int.Parse(scalar.Value, numberFormat);
					break;
				case TypeCode.Int64:
					value = long.Parse(scalar.Value, numberFormat);
					break;
				case TypeCode.SByte:
					value = sbyte.Parse(scalar.Value, numberFormat);
					break;
				case TypeCode.UInt16:
					value = ushort.Parse(scalar.Value, numberFormat);
					break;
				case TypeCode.UInt32:
					value = uint.Parse(scalar.Value, numberFormat);
					break;
				case TypeCode.UInt64:
					value = ulong.Parse(scalar.Value, numberFormat);
					break;
				case TypeCode.Single:
					value = float.Parse(scalar.Value, numberFormat);
					break;
				case TypeCode.Double:
					value = double.Parse(scalar.Value, numberFormat);
					break;
				case TypeCode.Decimal:
					value = decimal.Parse(scalar.Value, numberFormat);
					break;
				case TypeCode.String:
					value = scalar.Value;
					break;
				case TypeCode.Char:
					value = scalar.Value[0];
					break;
				case TypeCode.DateTime:
					value = DateTime.Parse(scalar.Value, CultureInfo.InvariantCulture);
					break;
				default:
					if (expectedType == typeof(object))
					{
						value = scalar.Value;
					}
					else
					{
						value = TypeConverter.ChangeType(scalar.Value, expectedType);
					}
					break;
				}
			}
			return true;
		}
	}
}
