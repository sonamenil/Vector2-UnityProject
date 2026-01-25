using System;

namespace YamlDotNet.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	[Obsolete("Please use YamlMember instead")]
	public class YamlAliasAttribute : Attribute
	{
		public string Alias { get; set; }

		public YamlAliasAttribute(string alias)
		{
			Alias = alias;
		}
	}
}
