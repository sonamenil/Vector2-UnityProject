using System;
using System.Collections;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace Nekki.Yaml
{
	[Serializable]
	public class Sequence : Node
	{
		private YamlSequenceNode _sequence;

		public List<Node> nodesInside { get; private set; }

		public Sequence(string keyNew, YamlSequenceNode sequenceNew)
		{
			base.typeNode = "Sequence";
			base.key = keyNew;
			base.value = sequenceNew;
			_sequence = (YamlSequenceNode)base.value;
			nodesInside = new List<Node>();
			foreach (YamlNode item in _sequence)
			{
				nodesInside.Add(Node.CreateNodeByType(base.key, item));
			}
		}

		public Sequence(string keyNew, Node node)
		{
			base.typeNode = "Sequence";
			base.key = keyNew;
			base.value = new YamlSequenceNode(new YamlNode[0]);
			_sequence = (YamlSequenceNode)base.value;
			nodesInside = new List<Node>();
			AddNode(node);
			foreach (YamlNode item in _sequence)
			{
				nodesInside.Add(Node.CreateNodeByType(base.key, item));
			}
		}

		public Sequence(string keyNew, Node[] mappingInside)
		{
			base.typeNode = "Sequence";
			base.key = keyNew;
			base.value = new YamlSequenceNode(new YamlNode[0]);
			_sequence = (YamlSequenceNode)base.value;
			nodesInside = new List<Node>();
			AddNodes(mappingInside);
		}

		public Sequence(string keyNew, List<Node> mappingInside)
			: this(keyNew, mappingInside.ToArray())
		{
			base.typeNode = "Sequence";
		}

		public void UpdateNodeAtIndex(int index, Node newNode)
		{
			_sequence.UpdateNode(nodesInside[index].value, newNode.value);
			nodesInside[index] = newNode;
		}

		public void Replace(List<Node> newNodes)
		{
			foreach (Node item in nodesInside)
			{
				_sequence.Remove(item.value);
			}
			foreach (Node newNode in newNodes)
			{
				_sequence.Add(newNode.value);
			}
			nodesInside = newNodes;
		}

		public void Remove(Node newNode)
		{
			_sequence.Remove(newNode.value);
			nodesInside.Remove(newNode);
		}

		public void AddNode(Node newNode)
		{
			_sequence.Add(newNode.value);
			nodesInside.Add(newNode);
		}

		public void AddNodes(Node[] newNodes)
		{
			foreach (Node node in newNodes)
			{
				_sequence.Add(node.value);
				nodesInside.Add(node);
			}
		}

		public void AddNodes(List<Node> newNodes)
		{
			foreach (Node newNode in newNodes)
			{
				_sequence.Add(newNode.value);
				nodesInside.Add(newNode);
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

		public override IEnumerator GetEnumerator()
		{
			foreach (Node item in nodesInside)
			{
				yield return item;
			}
		}
	}
}
