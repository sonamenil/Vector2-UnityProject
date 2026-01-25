using System;

namespace YamlDotNet.Serialization
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class YamlMemberAttribute : Attribute
	{
		public Type SerializeAs { get; set; }

		public int Order { get; set; }

		public string Alias { get; set; }

		public YamlMemberAttribute()
		{
		}

		public YamlMemberAttribute(Type serializeAs)
		{
			SerializeAs = serializeAs;
		}
	}
}
