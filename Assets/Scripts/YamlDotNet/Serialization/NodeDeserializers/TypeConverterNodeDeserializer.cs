using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class TypeConverterNodeDeserializer : INodeDeserializer
	{
		[CompilerGenerated]
		private sealed class Deserialize_003Ec__AnonStorey111
		{
			internal Type expectedType;

			internal bool _003C_003Em__1B0(IYamlTypeConverter c)
			{
				return c.Accepts(expectedType);
			}
		}

		private readonly IEnumerable<IYamlTypeConverter> converters;

		public TypeConverterNodeDeserializer(IEnumerable<IYamlTypeConverter> converters)
		{
			if (converters == null)
			{
				throw new ArgumentNullException("converters");
			}
			this.converters = converters;
		}

		bool INodeDeserializer.Deserialize(EventReader reader, Type expectedType, Func<EventReader, Type, object> nestedObjectDeserializer, out object value)
		{
			IYamlTypeConverter yamlTypeConverter = converters.FirstOrDefault((IYamlTypeConverter c) => c.Accepts(expectedType));
			if (yamlTypeConverter == null)
			{
				value = null;
				return false;
			}
			value = yamlTypeConverter.ReadYaml(reader.Parser, expectedType);
			return true;
		}
	}
}
