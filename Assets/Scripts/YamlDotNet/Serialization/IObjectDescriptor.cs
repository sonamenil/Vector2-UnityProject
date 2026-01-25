using System;

namespace YamlDotNet.Serialization
{
	public interface IObjectDescriptor
	{
		object Value { get; }

		Type Type { get; }

		Type StaticType { get; }
	}
}
