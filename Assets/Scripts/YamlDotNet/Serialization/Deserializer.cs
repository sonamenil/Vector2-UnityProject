using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.NodeDeserializers;
using YamlDotNet.Serialization.NodeTypeResolvers;
using YamlDotNet.Serialization.ObjectFactories;
using YamlDotNet.Serialization.TypeInspectors;
using YamlDotNet.Serialization.TypeResolvers;
using YamlDotNet.Serialization.Utilities;
using YamlDotNet.Serialization.ValueDeserializers;

namespace YamlDotNet.Serialization
{
	public sealed class Deserializer
	{
		private class TypeDescriptorProxy : ITypeInspector
		{
			public ITypeInspector TypeDescriptor;

			public IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
			{
				return TypeDescriptor.GetProperties(type, container);
			}

			public IPropertyDescriptor GetProperty(Type type, object container, string name, bool ignoreUnmatched)
			{
				return TypeDescriptor.GetProperty(type, container, name, ignoreUnmatched);
			}
		}

		private static readonly Dictionary<string, Type> predefinedTagMappings = new Dictionary<string, Type>
		{
			{
				"tag:yaml.org,2002:map",
				typeof(Dictionary<object, object>)
			},
			{
				"tag:yaml.org,2002:bool",
				typeof(bool)
			},
			{
				"tag:yaml.org,2002:float",
				typeof(double)
			},
			{
				"tag:yaml.org,2002:int",
				typeof(int)
			},
			{
				"tag:yaml.org,2002:str",
				typeof(string)
			},
			{
				"tag:yaml.org,2002:timestamp",
				typeof(DateTime)
			}
		};

		private readonly Dictionary<string, Type> tagMappings;

		private readonly List<IYamlTypeConverter> converters;

		private TypeDescriptorProxy typeDescriptor = new TypeDescriptorProxy();

		private IValueDeserializer valueDeserializer;

		public IList<INodeDeserializer> NodeDeserializers { get; private set; }

		public IList<INodeTypeResolver> TypeResolvers { get; private set; }

		public Deserializer(IObjectFactory objectFactory = null, INamingConvention namingConvention = null, bool ignoreUnmatched = false)
		{
			objectFactory = objectFactory ?? new DefaultObjectFactory();
			namingConvention = namingConvention ?? new NullNamingConvention();
			typeDescriptor.TypeDescriptor = new YamlAttributesTypeInspector(new NamingConventionTypeInspector(new ReadableAndWritablePropertiesTypeInspector(new ReadablePropertiesTypeInspector(new StaticTypeResolver())), namingConvention));
			converters = new List<IYamlTypeConverter>();
			foreach (IYamlTypeConverter builtInConverter in YamlTypeConverters.BuiltInConverters)
			{
				converters.Add(builtInConverter);
			}
			NodeDeserializers = new List<INodeDeserializer>();
			NodeDeserializers.Add(new TypeConverterNodeDeserializer(converters));
			NodeDeserializers.Add(new NullNodeDeserializer());
			NodeDeserializers.Add(new ScalarNodeDeserializer());
			NodeDeserializers.Add(new ArrayNodeDeserializer());
			NodeDeserializers.Add(new GenericDictionaryNodeDeserializer(objectFactory));
			NodeDeserializers.Add(new NonGenericDictionaryNodeDeserializer(objectFactory));
			NodeDeserializers.Add(new GenericCollectionNodeDeserializer(objectFactory));
			NodeDeserializers.Add(new NonGenericListNodeDeserializer(objectFactory));
			NodeDeserializers.Add(new EnumerableNodeDeserializer());
			NodeDeserializers.Add(new ObjectNodeDeserializer(objectFactory, typeDescriptor, ignoreUnmatched));
			tagMappings = new Dictionary<string, Type>(predefinedTagMappings);
			TypeResolvers = new List<INodeTypeResolver>();
			TypeResolvers.Add(new TagNodeTypeResolver(tagMappings));
			TypeResolvers.Add(new TypeNameInTagNodeTypeResolver());
			TypeResolvers.Add(new DefaultContainersNodeTypeResolver());
			valueDeserializer = new AliasValueDeserializer(new NodeValueDeserializer(NodeDeserializers, TypeResolvers));
		}

		public void RegisterTagMapping(string tag, Type type)
		{
			tagMappings.Add(tag, type);
		}

		public void RegisterTypeConverter(IYamlTypeConverter typeConverter)
		{
			converters.Add(typeConverter);
		}

		public T Deserialize<T>(TextReader input)
		{
			return (T)Deserialize(input, typeof(T));
		}

		public object Deserialize(TextReader input)
		{
			return Deserialize(input, typeof(object));
		}

		public object Deserialize(TextReader input, Type type)
		{
			return Deserialize(new EventReader(new Parser(input)), type);
		}

		public T Deserialize<T>(EventReader reader)
		{
			return (T)Deserialize(reader, typeof(T));
		}

		public object Deserialize(EventReader reader)
		{
			return Deserialize(reader, typeof(object));
		}

		public object Deserialize(EventReader reader, Type type)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			bool flag = reader.Allow<StreamStart>() != null;
			bool flag2 = reader.Allow<DocumentStart>() != null;
			object result = null;
			if (!reader.Accept<DocumentEnd>() && !reader.Accept<StreamEnd>())
			{
				using (SerializerState serializerState = new SerializerState())
				{
					result = valueDeserializer.DeserializeValue(reader, type, serializerState, valueDeserializer);
					serializerState.OnDeserialization();
				}
			}
			if (flag2)
			{
				reader.Expect<DocumentEnd>();
			}
			if (flag)
			{
				reader.Expect<StreamEnd>();
			}
			return result;
		}
	}
}
