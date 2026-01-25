using System;
using System.Collections;
using YamlDotNet.RepresentationModel;

namespace Nekki.Yaml
{
	[Serializable]
	public abstract class Node : IEnumerable
	{
		public string key { get; protected set; }

		public YamlNode value { get; protected set; }

		public string typeNode { get; protected set; }

		public override string ToString()
		{
			return value.ToString();
		}

		public string GetTypeNode()
		{
			return typeNode;
		}

		public string GetKey()
		{
			return key;
		}

		public static Node CreateNodeByType(string nodeKey, YamlNode nodeValue)
		{
			Type type = nodeValue.GetType();
			if (type == typeof(YamlScalarNode))
			{
				return new Scalar(nodeKey, (YamlScalarNode)nodeValue);
			}
			if (type == typeof(YamlSequenceNode))
			{
				return new Sequence(nodeKey, (YamlSequenceNode)nodeValue);
			}
			if (type == typeof(YamlMappingNode))
			{
				return new Mapping(nodeKey, (YamlMappingNode)nodeValue);
			}
			return null;
		}

		public virtual IEnumerator GetEnumerator()
		{
			yield return this;
		}
	}
}
