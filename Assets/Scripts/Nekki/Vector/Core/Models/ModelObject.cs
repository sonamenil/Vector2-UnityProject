using System;
using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Detector;
using Nekki.Vector.Core.Node;
using UnityEngine;

namespace Nekki.Vector.Core.Models
{
	public class ModelObject
	{
		private Model _Parent;

		private GameObject _Container = new GameObject
		{
			name = "[Model]"
		};

		private List<ModelRender> _Renders = new List<ModelRender>();

		private GameObject _Layer;

		private bool _IsEnabled;

		private bool _IsDebug;

		private Color _Color;

		private List<ModelNode> _CenterOfMass = new List<ModelNode>();

		private List<ModelNode> _Nodes = new List<ModelNode>();

		private List<ModelNode> _MacroNodes = new List<ModelNode>();

		private List<ModelNode> _NodesAll = new List<ModelNode>();

		private List<ModelNode> _CollisibleNodes = new List<ModelNode>();

		private List<ModelLine> _Edges = new List<ModelLine>();

		private List<ModelLine> _Muscules = new List<ModelLine>();

		private List<ModelLine> _EdgesAll = new List<ModelLine>();

		private List<ModelLine> _CollisibleEdges = new List<ModelLine>();

		private List<ModelLine> _Capsules = new List<ModelLine>();

		private float _DeltaBox = 200f;

		private Rectangle _Rectangle = new Rectangle(0f, 0f, 0f, 0f);

		private Rectangle _BoundingBox;

		private ModelNode _PivotNode;

		private ModelNode _DetectorHorizontalNode;

		private ModelNode _DetectorVerticalNode;

		private DetectorLine _DetectorVerticalLine;

		private DetectorLine _DetectorHorizontalLine;

		private ModelNode _CenterOfMassNode;

		private ModelNode _ToeRight;

		private ModelNode _ToeLeft;

		private CameraNode _CameraNode;

		private bool _IsAuxiliary = true;

		private List<int[]> _BothNodeList;

		public Model Parent
		{
			get
			{
				return _Parent;
			}
			set
			{
				_Parent = value;
				_Renders[0].Add(_Parent);
			}
		}

		public List<ModelRender> ModelRenders
		{
			get
			{
				return _Renders;
			}
		}

		public GameObject Layer
		{
			get
			{
				return _Layer;
			}
			set
			{
				_Layer = value;
				if (_Layer == null)
				{
					_Container.transform.parent = null;
					return;
				}
				Vector3 localPosition = _Container.transform.localPosition;
				_Container.transform.parent = _Layer.transform;
				_Container.transform.localPosition = localPosition;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return _IsEnabled;
			}
			set
			{
				_IsEnabled = value;
				_Container.SetActive(value);
			}
		}

		public bool IsDebug
		{
			get
			{
				return _IsDebug;
			}
			set
			{
				_IsDebug = value;
			}
		}

		public Color Color
		{
			get
			{
				return _Color;
			}
			set
			{
				_Color = value;
				foreach (ModelRender render in _Renders)
				{
					render.Color = value;
				}
			}
		}

		public string Name
		{
			get
			{
				return _Container.name;
			}
			set
			{
				_Container.name = "[Model] " + value;
			}
		}

		public List<ModelNode> CenterOfMass
		{
			get
			{
				return _CenterOfMass;
			}
		}

		public List<ModelNode> Nodes
		{
			get
			{
				return _Nodes;
			}
		}

		public List<ModelNode> MacroNodes
		{
			get
			{
				return _MacroNodes;
			}
		}

		public List<ModelNode> NodesAll
		{
			get
			{
				return _NodesAll;
			}
		}

		public List<ModelNode> CollisibleNodes
		{
			get
			{
				return _CollisibleNodes;
			}
		}

		public List<ModelLine> Edges
		{
			get
			{
				return _Edges;
			}
		}

		public List<ModelLine> Muscules
		{
			get
			{
				return _Muscules;
			}
		}

		public List<ModelLine> EdgesAll
		{
			get
			{
				return _EdgesAll;
			}
		}

		public List<ModelLine> CollisibleEdges
		{
			get
			{
				return _CollisibleEdges;
			}
		}

		public List<ModelLine> Capsules
		{
			get
			{
				return _Capsules;
			}
		}

		public float DeltaBox
		{
			get
			{
				return _DeltaBox;
			}
			set
			{
				_DeltaBox = value;
			}
		}

		public Rectangle Rectangle
		{
			get
			{
				_Rectangle.Origin.X = 0f - _DeltaBox;
				_Rectangle.Origin.Y = 0f - _DeltaBox;
				_Rectangle.Size.Width = _DeltaBox * 2f;
				_Rectangle.Size.Height = _DeltaBox * 2f;
				return _Rectangle;
			}
		}

		public Rectangle BoundingBox
		{
			get
			{
				return _BoundingBox;
			}
			set
			{
				_BoundingBox = value;
			}
		}

		public ModelNode PivotNode
		{
			get
			{
				return _PivotNode;
			}
		}

		public ModelNode DetectorHorizontalNode
		{
			get
			{
				return _DetectorHorizontalNode;
			}
		}

		public ModelNode DetectorVerticalNode
		{
			get
			{
				return _DetectorVerticalNode;
			}
		}

		public DetectorLine DetectorVerticalLine
		{
			get
			{
				return _DetectorVerticalLine;
			}
		}

		public DetectorLine DetectorHorizontalLine
		{
			get
			{
				return _DetectorHorizontalLine;
			}
		}

		public ModelNode CenterOfMassNode
		{
			get
			{
				return _CenterOfMassNode;
			}
		}

		public ModelNode ToeRight
		{
			get
			{
				return _ToeRight;
			}
		}

		public ModelNode ToeLeft
		{
			get
			{
				return _ToeLeft;
			}
		}

		public CameraNode CameraNode
		{
			get
			{
				return _CameraNode;
			}
		}

		public bool IsAuxiliary
		{
			get
			{
				return _IsAuxiliary;
			}
			set
			{
				_IsAuxiliary = value;
			}
		}

		public Vector3f Velocity
		{
			get
			{
				if (_CenterOfMassNode == null)
				{
					return new Vector3f(0f, 0f, 0f);
				}
				return _CenterOfMassNode.Start - _CenterOfMassNode.End;
			}
		}

		public List<int[]> BothNodeList
		{
			get
			{
				return _BothNodeList;
			}
		}

		public ModelObject(List<string> p_skins)
		{
			IsEnabled = false;
			Parse(p_skins);
			CreateBothNodeList();
			_Container.transform.localPosition = new Vector2(0f, 0f);
		}

		public void Parse(List<string> p_skins)
		{
			_CenterOfMass.Clear();
			_Nodes.Clear();
			_MacroNodes.Clear();
			_NodesAll.Clear();
			_CollisibleNodes.Clear();
			_Edges.Clear();
			_Muscules.Clear();
			_EdgesAll.Clear();
			_CollisibleEdges.Clear();
			_Capsules.Clear();
			foreach (string p_skin in p_skins)
			{
				ParseFile(p_skin);
			}
			_CameraNode = null;
			_PivotNode = GetNode("NPivot");
			_DetectorHorizontalNode = GetNode("DetectorH");
			_DetectorVerticalNode = GetNode("DetectorV");
			if (_DetectorHorizontalNode == null)
			{
				_DetectorHorizontalLine = null;
			}
			else
			{
				_DetectorHorizontalNode.IsDetector = false;
				_DetectorHorizontalLine = new DetectorLine(_DetectorHorizontalNode, DetectorLine.DetectorType.Horizontal)
				{
					Layer = _Container
				};
			}
			if (_DetectorVerticalNode == null)
			{
				_DetectorVerticalLine = null;
			}
			else
			{
				_DetectorVerticalNode.IsDetector = false;
				_DetectorVerticalLine = new DetectorLine(_DetectorVerticalNode, DetectorLine.DetectorType.Vertical)
				{
					Layer = _Container
				};
			}
			_CenterOfMassNode = GetNode("COM");
			_ToeRight = GetNode("NToe_1");
			_ToeLeft = GetNode("NToe_2");
			_CameraNode = new CameraNode(GetNode("Camera"));
			UpdateBoundingBox();
		}

		public void UpdateBoundingBox()
		{
			RenderMacroNode();
			_BoundingBox = new Rectangle(float.NaN, float.NaN, float.NaN, float.NaN);
			ModelNode modelNode = null;
			for (int i = 0; i < _NodesAll.Count; i++)
			{
				modelNode = _NodesAll[i];
				if (float.IsNaN(_BoundingBox.Origin.X) || modelNode.Start.X < _BoundingBox.Origin.X)
				{
					_BoundingBox.Origin.X = modelNode.Start.X;
				}
				if (float.IsNaN(_BoundingBox.Origin.Y) || modelNode.Start.Y < _BoundingBox.Origin.Y)
				{
					_BoundingBox.Origin.Y = modelNode.Start.Y;
				}
				if (float.IsNaN(_BoundingBox.Size.Width) || modelNode.Start.X > _BoundingBox.MaxX)
				{
					_BoundingBox.Size.Width = modelNode.Start.X - _BoundingBox.Origin.X;
				}
				if (float.IsNaN(_BoundingBox.Size.Height) || modelNode.Start.Y > _BoundingBox.MaxY)
				{
					_BoundingBox.Size.Height = modelNode.Start.Y - _BoundingBox.Origin.Y;
				}
			}
		}

		private void ParseFile(string p_file)
		{
			ModelRender modelRender = new ModelRender();
			modelRender.Name = "[Skins] " + p_file;
			modelRender.Layer = _Container;
			ModelRender modelRender2 = modelRender;
			XmlNode xmlNode = XmlUtils.OpenXMLDocument(VectorPaths.Models, p_file)["Scene"];
			if (xmlNode == null)
			{
				throw new Exception();
			}
			ParseNodes(xmlNode["Nodes"], modelRender2);
			ParseEdges(xmlNode["Edges"], modelRender2);
			ParseCapsules(xmlNode["Figures"], modelRender2);
			modelRender2.Init();
			_Renders.Add(modelRender2);
		}

		public void ParseNodes(XmlNode p_nodes, ModelRender p_render)
		{
			if (p_nodes == null)
			{
				return;
			}
			foreach (XmlNode childNode in p_nodes.ChildNodes)
			{
				ModelNode modelNode = ParseNode(childNode);
				switch (modelNode.Type)
				{
				case "Node":
					_Nodes.Add(modelNode);
					break;
				case "MacroNode":
					_MacroNodes.Add(modelNode);
					break;
				case "CenterOfMass":
					_CenterOfMass.Add(modelNode);
					break;
				}
				if (modelNode.IsCollisible)
				{
					_CollisibleNodes.Add(modelNode);
				}
				modelNode.Id = _NodesAll.Count;
				_NodesAll.Add(modelNode);
				p_render.Add(modelNode);
			}
		}

		private ModelNode ParseNode(XmlNode p_node)
		{
			float p_x = XmlUtils.ParseFloat(p_node.Attributes["X"]);
			float num = XmlUtils.ParseFloat(p_node.Attributes["Y"]);
			float p_z = XmlUtils.ParseFloat(p_node.Attributes["Z"]);
			string value = p_node.Attributes["Type"].Value;
			MacroNode p_macroNode = ((!(value == "MacroNode")) ? null : new MacroNode());
			ModelNode modelNode = new ModelNode(new Vector3f(p_x, 0f - num, p_z), p_macroNode);
			modelNode.Name = p_node.Name;
			modelNode.Type = value;
			modelNode.Weight = XmlUtils.ParseFloat(p_node.Attributes["Mass"]);
			modelNode.IsFixed = XmlUtils.ParseBool(p_node.Attributes["Fixed"]);
			modelNode.IsCollisible = XmlUtils.ParseBool(p_node.Attributes["Collisible"]);
			modelNode.IsPhysics = XmlUtils.ParseBool(p_node.Attributes["Cloth"]);
			modelNode.Attenuation = XmlUtils.ParseFloat(p_node.Attributes["Attenuation"]);
			switch (modelNode.Type)
			{
			case "CenterOfMass":
			{
				int num2 = XmlUtils.ParseInt(p_node.Attributes["NodesCount"]);
				if (num2 != 0)
				{
					modelNode.MacroNode = new MacroNode();
					for (int j = 0; j < num2; j++)
					{
						string value3 = p_node.Attributes["ChildNode" + (j + 1)].Value;
						modelNode.MacroNode.ChildNode.Add(GetNode(value3));
					}
				}
				break;
			}
			case "MacroNode":
			{
				int num2 = XmlUtils.ParseInt(p_node.Attributes["NodesCount"]);
				if (num2 != 0)
				{
					for (int i = 0; i < num2; i++)
					{
						string value2 = p_node.Attributes["ChildNode" + (i + 1)].Value;
						modelNode.MacroNode.ChildNode.Add(GetNode(value2));
						modelNode.MacroNode.LCC.Add(XmlUtils.ParseFloat(p_node.Attributes["LCC" + (i + 1)]));
					}
				}
				break;
			}
			}
			return modelNode;
		}

		public void ParseEdges(XmlNode p_node, ModelRender p_render)
		{
			if (p_node == null)
			{
				return;
			}
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				ModelLine modelLine = ParseEdge(childNode);
				switch (modelLine.Type)
				{
				case "Edge":
					_Edges.Add(modelLine);
					break;
				case "Muscle":
					_Muscules.Add(modelLine);
					break;
				}
				if (modelLine.Collisible)
				{
					_CollisibleEdges.Add(modelLine);
				}
				_EdgesAll.Add(modelLine);
				p_render.Add(modelLine);
			}
		}

		private ModelLine ParseEdge(XmlNode p_node)
		{
			string value = p_node.Attributes["End1"].Value;
			string value2 = p_node.Attributes["End2"].Value;
			ModelLine modelLine = new ModelLine(GetNode(value), GetNode(value2));
			modelLine.Name = p_node.Name;
			modelLine.Type = p_node.Attributes["Type"].Value;
			modelLine.Length = XmlUtils.ParseFloat(p_node.Attributes["Length"]);
			modelLine.Collisible = XmlUtils.ParseBool(p_node.Attributes["Collisible"]);
			return modelLine;
		}

		public void ParseCapsules(XmlNode p_node, ModelRender p_render)
		{
			if (p_node == null)
			{
				return;
			}
			foreach (XmlNode childNode in p_node.ChildNodes)
			{
				ParseCapsule(childNode, p_render);
			}
		}

		private void ParseCapsule(XmlNode p_node, ModelRender p_render)
		{
			switch (p_node.Attributes["Type"].Value)
			{
			case "Capsule":
			{
				string value = p_node.Attributes["Edge"].Value;
				ModelLine modelLine = new ModelLine(GetEdge(value));
				modelLine.Stroke = XmlUtils.ParseFloat(p_node.Attributes["Radius1"]);
				modelLine.Margin1 = XmlUtils.ParseFloat(p_node.Attributes["Margin1"]);
				modelLine.Margin2 = XmlUtils.ParseFloat(p_node.Attributes["Margin2"]);
				modelLine.Type = "Capsule";
				_Capsules.Add(modelLine);
				p_render.Add(modelLine);
				break;
			}
			case "Triangle":
				p_render.Add(GetNode(p_node.Attributes["Node1"].Value), GetNode(p_node.Attributes["Node2"].Value), GetNode(p_node.Attributes["Node3"].Value));
				break;
			}
		}

		public void PositionToPivot(ModelNode p_node)
		{
			if (p_node != null)
			{
				Vector3f vector3f = new Vector3f(-1f, -1f, 0f);
				p_node.Start.Set(_PivotNode.Start + vector3f);
				p_node.End.Set(_PivotNode.End + vector3f);
			}
		}

		public void Position(Vector3f p_vector, string p_name = "NPivot")
		{
			ModelNode node = GetNode(p_name);
			if (node == null)
			{
				return;
			}
			Vector3f p_vector2 = p_vector - node.Start;
			Vector3f p_vector3 = p_vector - node.End;
			foreach (ModelNode item in _NodesAll)
			{
				item.Start.Add(p_vector2);
				item.End.Add(p_vector3);
			}
			if (_DetectorHorizontalLine != null)
			{
				_DetectorHorizontalLine.Reset();
			}
			if (_DetectorVerticalLine != null)
			{
				_DetectorVerticalLine.Reset();
			}
		}

		private void CreateBothNodeList()
		{
			_BothNodeList = new List<int[]>();
			ModelNode modelNode = null;
			for (int i = 0; i < _NodesAll.Count; i++)
			{
				modelNode = _NodesAll[i];
				if (modelNode.BothIndex != 0 && !IsBothNodes(modelNode.Id, _BothNodeList))
				{
					_BothNodeList.Add(new int[2]
					{
						modelNode.Id,
						BothNode(modelNode, _NodesAll).Id
					});
				}
			}
		}

		private static bool IsBothNodes(int p_id, List<int[]> p_list)
		{
			for (int i = 0; i < p_list.Count; i++)
			{
				int[] array = p_list[i];
				if (p_id == array[0] || p_id == array[1])
				{
					return true;
				}
			}
			return false;
		}

		private static ModelNode BothNode(ModelNode p_node, List<ModelNode> p_nodes)
		{
			ModelNode modelNode = null;
			for (int i = 0; i < p_nodes.Count; i++)
			{
				modelNode = p_nodes[i];
				if (modelNode.Name != p_node.Name && modelNode.BothName == p_node.BothName)
				{
					return modelNode;
				}
			}
			return null;
		}

		public ModelNode GetNode(string p_name = "NPivot")
		{
			for (int i = 0; i < _NodesAll.Count; i++)
			{
				if (_NodesAll[i].Name == p_name)
				{
					return _NodesAll[i];
				}
			}
			return null;
		}

		public ModelNode GetNodeToLow(string p_name)
		{
			for (int i = 0; i < _NodesAll.Count; i++)
			{
				if (_NodesAll[i].Name.ToLower() == p_name)
				{
					return _NodesAll[i];
				}
			}
			return null;
		}

		public ModelNode GetNode(int p_id)
		{
			if (_NodesAll.Count <= p_id)
			{
				return null;
			}
			return _NodesAll[p_id];
		}

		public int GetNodeIdByName(string p_name = "NPivot")
		{
			foreach (ModelNode item in _NodesAll)
			{
				if (item.Name == p_name)
				{
					return item.Id;
				}
			}
			return -1;
		}

		public ModelLine GetEdge(string p_name)
		{
			foreach (ModelLine item in _EdgesAll)
			{
				if (item.Name == p_name)
				{
					return item;
				}
			}
			return null;
		}

		public Vector3f Position(string p_name = "NPivot", bool p_isCurrent = true)
		{
			ModelNode node = GetNode(p_name);
			if (node != null)
			{
				return (!p_isCurrent) ? node.End : node.Start;
			}
			return null;
		}

		public void Reset()
		{
			foreach (ModelNode item in _NodesAll)
			{
				item.Reset();
			}
		}

		public void RenderMacroNode()
		{
			for (int i = 0; i < _MacroNodes.Count; i++)
			{
				_MacroNodes[i].MacroNodeCompute();
			}
		}

		public void RenderDetector()
		{
			if (_DetectorVerticalLine != null)
			{
				_DetectorVerticalLine.Update();
			}
			if (_DetectorHorizontalLine != null)
			{
				_DetectorHorizontalLine.Update();
			}
		}

		public void Render()
		{
			for (int i = 0; i < _Renders.Count; i++)
			{
				_Renders[i].Render();
			}
		}
	}
}
