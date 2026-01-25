using System;
using System.Collections;
using System.Runtime.CompilerServices;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class NonGenericDictionaryNodeDeserializer : INodeDeserializer
	{
		[CompilerGenerated]
		private sealed class Deserialize_003Ec__AnonStorey10A
		{
			internal IDictionary dictionary;
		}

		[CompilerGenerated]
		private sealed class Deserialize_003Ec__AnonStorey10B
		{
			internal object key;

			internal object keyValue;

			internal Deserialize_003Ec__AnonStorey10A _003C_003Ef__ref_0024266;

			internal void _003C_003Em__1AA(object v)
			{
				_003C_003Ef__ref_0024266.dictionary.Add(key, v);
			}

			internal void _003C_003Em__1AB(object v)
			{
				_003C_003Ef__ref_0024266.dictionary.Add(v, keyValue);
			}
		}

		[CompilerGenerated]
		private sealed class Deserialize_003Ec__AnonStorey10C
		{
			internal bool hasFirstPart;

			internal Deserialize_003Ec__AnonStorey10A _003C_003Ef__ref_0024266;

			internal Deserialize_003Ec__AnonStorey10B _003C_003Ef__ref_0024267;

			internal void _003C_003Em__1AC(object v)
			{
				if (hasFirstPart)
				{
					_003C_003Ef__ref_0024266.dictionary.Add(v, _003C_003Ef__ref_0024267.keyValue);
					return;
				}
				_003C_003Ef__ref_0024267.key = v;
				hasFirstPart = true;
			}

			internal void _003C_003Em__1AD(object v)
			{
				if (hasFirstPart)
				{
					_003C_003Ef__ref_0024266.dictionary.Add(_003C_003Ef__ref_0024267.key, v);
					return;
				}
				_003C_003Ef__ref_0024267.keyValue = v;
				hasFirstPart = true;
			}
		}

		private readonly IObjectFactory _objectFactory;

		public NonGenericDictionaryNodeDeserializer(IObjectFactory objectFactory)
		{
			_objectFactory = objectFactory;
		}

		bool INodeDeserializer.Deserialize(EventReader reader, Type expectedType, Func<EventReader, Type, object> nestedObjectDeserializer, out object value)
		{
			if (!typeof(IDictionary).IsAssignableFrom(expectedType))
			{
				value = false;
				return false;
			}
			reader.Expect<MappingStart>();
			IDictionary dictionary = (IDictionary)_objectFactory.Create(expectedType);
			while (!reader.Accept<MappingEnd>())
			{
				object key = nestedObjectDeserializer(reader, typeof(object));
				IValuePromise valuePromise = key as IValuePromise;
				object keyValue = nestedObjectDeserializer(reader, typeof(object));
				IValuePromise valuePromise2 = keyValue as IValuePromise;
				if (valuePromise == null)
				{
					if (valuePromise2 == null)
					{
						dictionary.Add(key, keyValue);
						continue;
					}
					valuePromise2.ValueAvailable += delegate(object v)
					{
						dictionary.Add(key, v);
					};
					continue;
				}
				if (valuePromise2 == null)
				{
					valuePromise.ValueAvailable += delegate(object v)
					{
						dictionary.Add(v, keyValue);
					};
					continue;
				}
				bool hasFirstPart = false;
				valuePromise.ValueAvailable += delegate(object v)
				{
					if (hasFirstPart)
					{
						dictionary.Add(v, keyValue);
					}
					else
					{
						key = v;
						hasFirstPart = true;
					}
				};
				valuePromise2.ValueAvailable += delegate(object v)
				{
					if (hasFirstPart)
					{
						dictionary.Add(key, v);
					}
					else
					{
						keyValue = v;
						hasFirstPart = true;
					}
				};
			}
			value = dictionary;
			reader.Expect<MappingEnd>();
			return true;
		}
	}
}
