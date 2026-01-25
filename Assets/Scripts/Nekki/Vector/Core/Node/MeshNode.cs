using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Vector.Core.Node
{
	public class MeshNode
	{
		public int[] Triangles;

		public Vector3[] Vertices;

		private List<ModelNode> _VertecesNode = new List<ModelNode>();

		private List<int> _TrianglesList = new List<int>();

		public void AddTriangle(ModelNode p_node1, ModelNode p_node2, ModelNode p_node3)
		{
			int nodeIndex = GetNodeIndex(p_node1);
			int nodeIndex2 = GetNodeIndex(p_node2);
			int nodeIndex3 = GetNodeIndex(p_node3);
			_TrianglesList.Add(nodeIndex);
			_TrianglesList.Add(nodeIndex2);
			_TrianglesList.Add(nodeIndex3);
		}

		private int GetNodeIndex(ModelNode p_node)
		{
			if (_VertecesNode.Contains(p_node))
			{
				return _VertecesNode.IndexOf(p_node);
			}
			_VertecesNode.Add(p_node);
			return _VertecesNode.Count - 1;
		}

		public void Init()
		{
			Triangles = _TrianglesList.ToArray();
			Vertices = new Vector3[_VertecesNode.Count];
			_TrianglesList = null;
		}

		public void Render()
		{
			Vector3f vector3f = null;
			for (int i = 0; i < Vertices.Length; i++)
			{
				vector3f = _VertecesNode[i].Start;
				Vertices[i].Set(vector3f.X, vector3f.Y, 0f);
			}
		}
	}
}
