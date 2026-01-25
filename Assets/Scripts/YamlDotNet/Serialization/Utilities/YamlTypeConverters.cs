using System.Collections.Generic;
using YamlDotNet.Serialization.Converters;

namespace YamlDotNet.Serialization.Utilities
{
	internal static class YamlTypeConverters
	{
		private static readonly IEnumerable<IYamlTypeConverter> _builtInTypeConverters = new IYamlTypeConverter[1]
		{
			new GuidConverter()
		};

		public static IEnumerable<IYamlTypeConverter> BuiltInConverters
		{
			get
			{
				return _builtInTypeConverters;
			}
		}
	}
}
