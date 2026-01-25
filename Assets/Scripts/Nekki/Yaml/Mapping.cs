using System;
using System.Collections;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace Nekki.Yaml
{
	[Serializable]
	public class Mapping : Node
	{
		private YamlMappingNode _mapping;

		public List<Node> nodesInside { get; private set; }

		public Mapping(string keyNew, Node[] entries)
		{
			base.typeNode = "Mapping";
			base.key = keyNew;
			base.value = new YamlMappingNode(new YamlNode[0]);
			_mapping = (YamlMappingNode)base.value;
			nodesInside = new List<Node>();
			foreach (Node node in entries)
			{
				_mapping.Add(node.key, node.value);
				nodesInside.Add(Node.CreateNodeByType(node.key, node.value));
			}
		}

		public Mapping(string keyNew, List<Node> entries)
			: this(keyNew, entries.ToArray())
		{
			base.typeNode = "Mapping";
		}

		public Mapping(Mapping entriesMapping)
			: this(entriesMapping.key, (YamlMappingNode)entriesMapping.value)
		{
			base.typeNode = "Mapping";
		}

		public Mapping(string keyNew, YamlMappingNode mapping)
		{
			base.typeNode = "Mapping";
			base.key = keyNew;
			base.value = mapping;
			_mapping = (YamlMappingNode)base.value;
			nodesInside = new List<Node>();
			foreach (KeyValuePair<YamlNode, YamlNode> item in _mapping)
			{
				nodesInside.Add(Node.CreateNodeByType(item.Key.ToString(), item.Value));
			}
		}

		public int GetNodesSize()
		{
			return nodesInside.Count;
		}

		public Node GetNodesByIndex(int index)
		{
			if (index < nodesInside.Count)
			{
				return nodesInside[index];
			}
			return null;
		}

		public List<Node> GetNodesInside()
		{
			return nodesInside;
		}

		public void Add(Node entry)
		{
			_mapping.Add(entry.key, entry.value);
			nodesInside.Add(entry);
		}

		public void AddNodes(Node[] newNodes)
		{
			foreach (Node node in newNodes)
			{
				_mapping.Add(node.key, node.value);
				nodesInside.Add(node);
			}
		}

		public void Remove(string key, string value)
		{
			foreach (Node item in nodesInside)
			{
				if (item.key == key && item.value.ToString() == value)
				{
					nodesInside.Remove(item);
					_mapping.Remove(key, item.value);
					break;
				}
			}
		}

		public void Remove(Node nodeToDelete)
		{
			if (nodeToDelete != null)
			{
				Remove(nodeToDelete.key, nodeToDelete.value.ToString());
			}
		}

		public Mapping GetMapping(string name)
		{
			if (!_mapping.HasKey(name))
			{
				return null;
			}
			YamlNode node = _mapping.GetNode(name);
			Type type = node.GetType();
			if (type == typeof(YamlMappingNode))
			{
				return new Mapping(name, (YamlMappingNode)node);
			}
			return null;
		}

		public Sequence GetSequence(string name)
		{
			if (!_mapping.HasKey(name))
			{
				return null;
			}
			Sequence result = null;
			foreach (Node item in nodesInside)
			{
				if (item.key.Equals(name) && item is Sequence)
				{
					result = (Sequence)item;
					break;
				}
			}
			return result;
		}

		public Scalar GetText(string name)
		{
			if (!_mapping.HasKey(name))
			{
				return null;
			}
			YamlNode node = _mapping.GetNode(name);
			Type type = node.GetType();
			if (type == typeof(YamlScalarNode))
			{
				return new Scalar(name, (YamlScalarNode)node);
			}
			return null;
		}

		public Node GetNode(string name)
		{
			if (!_mapping.HasKey(name))
			{
				return null;
			}
			YamlNode node = _mapping.GetNode(name);
			Type type = node.GetType();
			if (type == typeof(YamlScalarNode))
			{
				return new Scalar(name, (YamlScalarNode)node);
			}
			if (type == typeof(YamlSequenceNode))
			{
				return new Sequence(name, (YamlSequenceNode)node);
			}
			if (type == typeof(YamlMappingNode))
			{
				return new Mapping(name, (YamlMappingNode)node);
			}
			return null;
		}

		public override IEnumerator GetEnumerator()
		{
			foreach (Node item in nodesInside)
			{
				yield return item;
			}
		}
	}
}
