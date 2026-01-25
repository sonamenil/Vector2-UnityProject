using System;
using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Runners.Animation;
using Nekki.Vector.Core.Scripts.Engine.Debug;
using Nekki.Vector.Core.Transformations;
using Nekki.Vector.Core.Utilites;
using UnityEngine;

namespace Nekki.Vector.Core.Runners
{
	public class ObjectRunner : TransformInterface
	{
		private const string _DefaultLocalNamespace = "UnknownObject";

		private ObjectRunner _Parent;

		protected List<ObjectRunner> _Childs = new List<ObjectRunner>();

		private Vector3f _DefaultPosition = new Vector3f(0f, 0f, 0f);

		private Dictionary<string, List<Transformation>> _TransformationDictionary = new Dictionary<string, List<Transformation>>();

		private float _FactorX;

		private float _FactorY;

		private int _AutoLayout = -1;

		private int _TransformObjectsCounter;

		private int _ObjectsToTransform;

		protected string _Name;

		private string _LocalNamespace;

		protected Element _Element;

		private Color _DefaultColor = new Color(-1f, 1f, 1f, 1f);

		private Color _Color = new Color(-1f, 1f, 1f, 1f);

		private static string _CurrentLocalNamespace = string.Empty;

		private bool _IsReference;

		private bool _IsMatrixUse;

		private Matrix4x4 _Transformation = Matrix4x4.identity;

		protected bool _IsDebug = true;

		private bool _MoveRoot;

		public ObjectRunner Parent
		{
			get
			{
				return _Parent;
			}
			set
			{
				_Parent = value;
				if (_Parent != null)
				{
					_CachedTransform.SetParent(_Parent.UnityObject.transform, false);
				}
			}
		}

		public ObjectRunner ParentRoot
		{
			get
			{
				if (_Parent == null)
				{
					return this;
				}
				return _Parent.ParentRoot;
			}
		}

		public List<ObjectRunner> Childs
		{
			get
			{
				return _Childs;
			}
		}

		public float FactorX
		{
			get
			{
				return _FactorX;
			}
			set
			{
				_FactorX = value;
			}
		}

		public float FactorY
		{
			get
			{
				return _FactorY;
			}
			set
			{
				_FactorY = value;
			}
		}

		public string Name
		{
			get
			{
				return _Name;
			}
		}

		public string LocalNamespace
		{
			get
			{
				return _LocalNamespace;
			}
		}

		public Element Element
		{
			get
			{
				return _Element;
			}
		}

		public Color DefaultColor
		{
			get
			{
				if (_DefaultColor.r < 0f && Parent != null)
				{
					return Parent.DefaultColor;
				}
				if (Parent != null && _Parent.DefaultColor.r >= 0f)
				{
					Color result = Color.Lerp(_Parent.DefaultColor, _DefaultColor, 0.5f);
					result.a = _Parent.DefaultColor.a * _DefaultColor.a;
					return result;
				}
				return _DefaultColor;
			}
		}

		public Color Color
		{
			get
			{
				if (_Color.r < 0f && Parent != null)
				{
					return Parent.Color;
				}
				if (Parent != null && _Parent.Color.r >= 0f)
				{
					Color result = Color.Lerp(_Parent.Color, _Color, 0.5f);
					result.a = _Parent.Color.a * _Color.a;
					return result;
				}
				return _Color;
			}
			set
			{
				_Color = value;
				if (_Color.r < 0f)
				{
					_Color.r = 0f;
				}
				for (int i = 0; i < _Element.Visuals.Count; i++)
				{
					_Element.Visuals[i].UpdateColor();
				}
				for (int j = 0; j < _Childs.Count; j++)
				{
					_Childs[j].Color = value;
				}
			}
		}

		public static string CurrentLocalNamespace
		{
			get
			{
				return _CurrentLocalNamespace;
			}
			set
			{
				_CurrentLocalNamespace = value;
			}
		}

		public bool IsReference
		{
			get
			{
				return _IsReference;
			}
		}

		private float MinX
		{
			get
			{
				float num = float.MaxValue;
				List<VisualRunner> visuals = _Element.Visuals;
				if (visuals.Count == 0 && _Childs.Count == 0)
				{
					return 0f;
				}
				for (int i = 0; i < visuals.Count; i++)
				{
					float x = visuals[i].LocalPosition.x;
					if (x < num)
					{
						num = x;
					}
				}
				for (int j = 0; j < _Childs.Count; j++)
				{
					float minX = _Childs[j].MinX;
					if (minX < num)
					{
						num = minX;
					}
				}
				return num;
			}
		}

		private float MaxX
		{
			get
			{
				float num = float.MinValue;
				List<VisualRunner> visuals = _Element.Visuals;
				if (visuals.Count == 0 && _Childs.Count == 0)
				{
					return 0f;
				}
				for (int i = 0; i < visuals.Count; i++)
				{
					float num2 = visuals[i].LocalPosition.x + visuals[i].ImageWidth;
					if (num2 > num)
					{
						num = num2;
					}
				}
				for (int j = 0; j < _Childs.Count; j++)
				{
					float maxX = _Childs[j].MaxX;
					if (maxX > num)
					{
						num = maxX;
					}
				}
				return num;
			}
		}

		public override bool IsEnabled
		{
			get
			{
				return base.IsEnabled;
			}
		}

		public bool IsEnableUnityGO
		{
			get
			{
				return base.UnityObject.activeSelf;
			}
			set
			{
				base.UnityObject.SetActive(value);
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
				for (int i = 0; i < _Childs.Count; i++)
				{
					_Childs[i].IsDebug = _IsDebug;
				}
				for (int j = 0; j < _Element.Elements.Count; j++)
				{
					_Element.Elements[j].IsDebug = _IsDebug;
				}
			}
		}

		public string GetCurrentLocalNamespace()
		{
			if (_LocalNamespace != null)
			{
				return _LocalNamespace;
			}
			return (_Parent == null) ? "UnknownObject" : _Parent.GetCurrentLocalNamespace();
		}

		public override void SetEnabled(bool p_enabled, bool p_restore = false, bool fromHierarchy = false)
		{
			base.SetEnabled(p_enabled, p_restore, fromHierarchy);
			if (_Element != null)
			{
				for (int i = 0; i < _Element.Elements.Count; i++)
				{
					_Element.Elements[i].SetEnabled(_IsEnabled, p_restore, true);
				}
			}
			for (int j = 0; j < _Childs.Count; j++)
			{
				_Childs[j].SetEnabled(_IsEnabled, p_restore, true);
			}
		}

		public virtual void Parse(XmlNode p_node, Dictionary<string, string> p_choices)
		{
			_Name = XmlUtils.ParseString(p_node.Attributes["Name"]);
			_UnityObject = new GameObject("Object: " + _Name);
			_CachedTransform = _UnityObject.transform;
			_CachedTransform.localPosition = new Vector3(XmlUtils.ParseFloat(p_node.Attributes["X"]), XmlUtils.ParseFloat(p_node.Attributes["Y"]));
			if (_Parent == null)
			{
				_FactorX = XmlUtils.ParseFloat(p_node.Attributes["Factor"]);
			}
			else
			{
				_FactorX = XmlUtils.ParseFloat(p_node.Attributes["Factor"], _Parent.FactorX);
			}
			_FactorY = _FactorX;
			_DefaultPosition.Set(_CachedTransform.localPosition);
			ParseTransformation(p_node);
			ParseMatrix(p_node);
			ParseEnable(p_node);
			_AutoLayout = XmlUtils.ParseInt(p_node.Attributes["AutoLayout"], -1);
			_LocalNamespace = XmlUtils.ParseString(p_node.Attributes["LocalNamespace"]);
			_MoveRoot = XmlUtils.ParseBool(p_node.Attributes["MoveRoot"]);
			CreateElement(p_node["Content"], p_choices);
			ParseColor(p_node);
		}

		public void AddChild(ObjectRunner runner)
		{
			_Childs.Add(runner);
			runner.Parent = this;
		}

		public void SetPositionByPlaceholder(Placeholder placeholder)
		{
			_CachedTransform.localPosition = placeholder.LocalPosition;
			_DefaultPosition = new Vector3f(placeholder.LocalPosition);
		}

		public void SetPosition(Vector3 p_position)
		{
			_CachedTransform.localPosition = p_position;
		}

		public override void SetPosition(float x, float y)
		{
			Vector3 localPosition = _CachedTransform.localPosition;
			localPosition.x = x;
			localPosition.y = y;
			_CachedTransform.localPosition = localPosition;
		}

		private void ParseEnable(XmlNode p_node)
		{
			if (p_node["Properties"] != null && p_node["Properties"]["Static"] != null && p_node["Properties"]["Static"]["Enable"] != null)
			{
				SetEnabled(XmlUtils.ParseBool(p_node["Properties"]["Static"]["Enable"].Attributes["Value"], true));
			}
		}

		private void ParseColor(XmlNode p_node)
		{
			if (p_node["Properties"] != null && p_node["Properties"]["Static"] != null && p_node["Properties"]["Static"]["StartColor"] != null)
			{
				_DefaultColor = ColorUtils.FromHex(p_node["Properties"]["Static"]["StartColor"].Attributes["Color"].Value);
				Color = _DefaultColor;
			}
		}

		private void ParseMatrix(XmlNode p_node)
		{
			if (p_node["Properties"] != null && p_node["Properties"]["Static"] != null && p_node["Properties"]["Static"]["Matrix"] != null)
			{
				_IsMatrixUse = true;
				p_node = p_node["Properties"]["Static"]["Matrix"];
				_Transformation[0, 0] = XmlUtils.ParseFloat(p_node.Attributes["A"], 1f);
				_Transformation[0, 1] = XmlUtils.ParseFloat(p_node.Attributes["B"]);
				_Transformation[1, 0] = XmlUtils.ParseFloat(p_node.Attributes["C"]);
				_Transformation[1, 1] = XmlUtils.ParseFloat(p_node.Attributes["D"], 1f);
				_Transformation[2, 2] = 1f;
				_Transformation[3, 3] = 1f;
				Vector3 localPosition = _CachedTransform.localPosition;
				localPosition.x += XmlUtils.ParseFloat(p_node.Attributes["Tx"]);
				localPosition.y += XmlUtils.ParseFloat(p_node.Attributes["Ty"]);
				_CachedTransform.localPosition = localPosition;
				if (_SupportUnityObject == null)
				{
					_SupportUnityObject = new GameObject
					{
						name = "Support Object"
					};
					_SupportUnityObject.transform.SetParent(_CachedTransform, false);
				}
			}
		}

		public virtual void Init()
		{
			ActivateMatrix();
			InitElements();
			ActivateAutoLayout();
			SetColor();
			for (int num = _Childs.Count - 1; num >= 0; num--)
			{
				_Childs[num].Init();
				BuildTranformationTable(_Childs[num].TransformationData);
			}
			BuildTranformationTable(base.TransformationData);
			if (!_IsEnabled)
			{
				SetEnabled(_IsEnabled);
			}
			if (_MoveRoot)
			{
				RunMainController.Location.Sets.MoveToRootObjects.Add(this);
			}
			InitTriggers();
			InitVisuals();
        }

		private void ActivateMatrix()
		{
			if (_IsMatrixUse)
			{
				AffineDecomposition affineDecomposition = new AffineDecomposition(_Transformation);
				_SupportUnityObject.transform.localScale = new Vector3(affineDecomposition.ScaleX1, affineDecomposition.ScaleY1, 1f);
				_SupportUnityObject.transform.Rotate(0f, 0f, affineDecomposition.Angle1, Space.Self);
				_CachedTransform.localScale = new Vector3(affineDecomposition.ScaleX2, affineDecomposition.ScaleY2, 1f);
				_CachedTransform.Rotate(0f, 0f, affineDecomposition.Angle2, Space.Self);
			}
		}

		private void ActivateAutoLayout()
		{
			if (_AutoLayout == -1)
			{
				return;
			}
			Vector3f vector3f = new Vector3f(0f, 0f, 0f);
			List<VisualRunner> visuals = _Element.Visuals;
			for (int i = 0; i < visuals.Count; i++)
			{
				visuals[i].Shift(vector3f);
				vector3f.X += visuals[i].ImageWidth + (float)_AutoLayout;
			}
			for (int j = 0; j < _Childs.Count; j++)
			{
				float minX = _Childs[j].MinX;
				float maxX = _Childs[j].MaxX;
				vector3f.X -= minX;
				_Childs[j].MoveLocalPosition(vector3f);
				if (minX != 0f || maxX != 0f)
				{
					vector3f.X += maxX + (float)_AutoLayout;
				}
			}
		}

		private void SetColor()
		{
			if (!(_DefaultColor.r < 0f))
			{
				Color = _DefaultColor;
			}
		}

		private void InitTriggers()
		{
			foreach (TriggerRunner trigger in _Element.Triggers)
			{
				trigger.Init();
			}
		}

		private void InitVisuals()
		{
			foreach (VisualRunner visual in _Element.Visuals)
			{
				visual.GenerateContent();
			}
			foreach (Nekki.Vector.Core.Runners.Animation.Animation animation in _Element.Animations)
			{
				animation.GenerateContent();
			}
			foreach (CustomAnimation customAnimation in _Element.CustomAnimations)
			{
				customAnimation.GenerateContent();
			}
		}

		protected static void CollectQuads(ObjectRunner p_object, List<QuadRunner> p_quads)
		{
			p_quads.AddRange(p_object._Element.QuadsAll);
			for (int i = 0; i < p_object._Childs.Count; i++)
			{
				CollectQuads(p_object._Childs[i], p_quads);
			}
		}

		public void CreateChild(XmlNode p_mainNode, Dictionary<string, string> p_Choices)
		{
			if (p_mainNode != null && (p_mainNode["Properties"] == null || Element.CheckSelection(p_mainNode["Properties"]["Static"], p_Choices)))
			{
				XmlNode xmlNode = null;
				if (p_mainNode.Attributes["Name"] != null && p_mainNode.Attributes["Filename"] != null)
				{
					xmlNode = Sets.ObjectNode(p_mainNode.Attributes["Name"].Value, p_mainNode.Attributes["Filename"].Value).GetXmlNode(p_mainNode, p_Choices);
				}
				ObjectRunner objectRunner = new ObjectRunner();
				objectRunner.Parse(p_mainNode, p_Choices);
				AddChild(objectRunner);
				if (xmlNode != null)
				{
					objectRunner._IsReference = true;
					objectRunner._UnityObject.name = "ObjectReference: " + objectRunner._Name;
					ObjectRunner objectRunner2 = new ObjectRunner();
					objectRunner2.Parse(xmlNode, p_Choices);
					objectRunner.AddChild(objectRunner2);
				}
			}
		}

		public void CreateElement(XmlNode p_node, Dictionary<string, string> p_choices)
		{
			if (_Element == null)
			{
				_Element = new Element(this);
			}
			_Element.Parse(p_node, p_choices);
		}

		public void InitElements()
		{
			if (_Element != null)
			{
				Runner runner = null;
				for (int i = 0; i < _Element.Elements.Count; i++)
				{
					runner = _Element.Elements[i];
					runner.InitRunner();
					BuildTranformationTable(runner.TransformationData);
				}
			}
		}

		public void BuildTranformationTable(List<Transformation> p_transforms)
		{
			if (p_transforms == null)
			{
				return;
			}
			List<Transformation> value = null;
			Transformation transformation = null;
			for (int i = 0; i < p_transforms.Count; i++)
			{
				transformation = p_transforms[i];
				if (!_TransformationDictionary.TryGetValue(transformation.Name, out value))
				{
					value = new List<Transformation>();
					_TransformationDictionary.Add(transformation.Name, value);
				}
				value.Add(transformation);
			}
		}

		public void MoveToRoot()
		{
			_UnityObject.transform.SetParent(null, true);
			_Parent.Childs.Remove(this);
			_Parent = null;
		}

		public static WaypointRunner GetWaypointByName(string p_name, ObjectRunner p_object)
		{
			List<WaypointRunner> waypoints = p_object._Element.Waypoints;
			for (int i = 0; i < waypoints.Count; i++)
			{
				if (waypoints[i].Name == p_name)
				{
					return waypoints[i];
				}
			}
			WaypointRunner waypointRunner = null;
			for (int j = 0; j < p_object._Childs.Count; j++)
			{
				waypointRunner = GetWaypointByName(p_name, p_object._Childs[j]);
				if (waypointRunner != null)
				{
					return waypointRunner;
				}
			}
			return null;
		}

		public int RunTranformation(string p_name)
		{
			if (_TransformationDictionary.ContainsKey(p_name))
			{
				List<Transformation> list = _TransformationDictionary[p_name];
				int num = 0;
				for (int i = 0; i < list.Count; i++)
				{
					num = Math.Max(num, list[i].Run());
				}
				return num;
			}
			int num2 = 0;
			for (int j = 0; j < _Childs.Count; j++)
			{
				int num3 = _Childs[j].RunTranformation(p_name);
				if (num3 != 0)
				{
					num2 = Math.Max(num3, num2);
				}
			}
			return num2;
		}

		public List<Transformation> GetTransformableObjects(string transformName)
		{
			List<Transformation> list = new List<Transformation>();
			if (_TransformationDictionary.ContainsKey(transformName))
			{
				list.AddRange(_TransformationDictionary[transformName]);
			}
			for (int i = 0; i < _Childs.Count; i++)
			{
				list.AddRange(_Childs[i].GetTransformableObjects(transformName));
			}
			return list;
		}

		public int RunTranformation(string p_name, int objectsToTransform)
		{
			List<Transformation> transformableObjects = GetTransformableObjects(p_name);
			System.Random random = new System.Random();
			for (int i = 0; i < transformableObjects.Count; i++)
			{
				int index = random.Next(i, transformableObjects.Count);
				Transformation value = transformableObjects[i];
				transformableObjects[i] = transformableObjects[index];
				transformableObjects[index] = value;
			}
			int num = 0;
			for (int j = 0; j < Math.Min(objectsToTransform, transformableObjects.Count); j++)
			{
				num = Math.Max(num, transformableObjects[j].Run());
			}
			return num;
		}

		public void StopTranformation(string p_name)
		{
			if (_TransformationDictionary.ContainsKey(p_name))
			{
				List<Transformation> list = _TransformationDictionary[p_name];
				for (int i = 0; i < list.Count; i++)
				{
					TransformationManager.Current.Remove(list[i]);
				}
			}
			else
			{
				for (int j = 0; j < _Childs.Count; j++)
				{
					_Childs[j].StopTranformation(p_name);
				}
			}
		}

		public int GetTransformationFrame(string p_name)
		{
			if (_TransformationDictionary.ContainsKey(p_name))
			{
				List<Transformation> list = _TransformationDictionary[p_name];
				int num = 0;
				for (int i = 0; i < list.Count; i++)
				{
					num = Math.Max(num, list[i].Frames);
				}
				return num;
			}
			int num2 = 0;
			for (int j = 0; j < _Childs.Count; j++)
			{
				int transformationFrame = _Childs[j].GetTransformationFrame(p_name);
				if (transformationFrame != 0)
				{
					num2 = Math.Max(transformationFrame, num2);
				}
			}
			return num2;
		}

		public void UpdatePosition(Vector3f p_delta)
		{
			Vector3 localPosition = _CachedTransform.localPosition;
			localPosition.x += p_delta.X;
			localPosition.y += p_delta.Y;
			_CachedTransform.localPosition = localPosition;
		}

		public void MoveLocalPosition(Vector3f p_delta)
		{
			UpdatePosition(p_delta);
			UpdatePositionOnObject(this, true);
		}

		private static void UpdatePositionOnObject(ObjectRunner p_object, bool resetSpeed = false)
		{
			List<Runner> elements = p_object._Element.Elements;
			Runner runner = null;
			for (int i = 0; i < elements.Count; i++)
			{
				runner = elements[i];
				runner.UpdatePosition();
				if (resetSpeed)
				{
					runner.TransformResetTween();
				}
			}
			List<ObjectRunner> childs = p_object._Childs;
			for (int j = 0; j < childs.Count; j++)
			{
				UpdatePositionOnObject(childs[j], resetSpeed);
			}
		}

		public override void TransformationStart()
		{
			List<Runner> elements = _Element.Elements;
			for (int i = 0; i < elements.Count; i++)
			{
				elements[i].TransformationStart();
			}
			for (int j = 0; j < _Childs.Count; j++)
			{
				_Childs[j].TransformationStart();
			}
		}

		public override void TransformationEnd()
		{
			List<Runner> elements = _Element.Elements;
			for (int i = 0; i < elements.Count; i++)
			{
				elements[i].TransformationEnd();
			}
			for (int j = 0; j < _Childs.Count; j++)
			{
				_Childs[j].TransformationEnd();
			}
		}

		public override void SetDeltaMove()
		{
			if (_IsDeltaMoveChange)
			{
				UpdatePosition(_DeltaMove);
				ResetDeltaMove();
			}
			UpdatePositionOnObject(this);
		}

		public override void TransformResetTween()
		{
			for (int i = 0; i < _Childs.Count; i++)
			{
				_Childs[i].TransformResetTween();
			}
			List<Runner> elements = _Element.Elements;
			for (int j = 0; j < elements.Count; j++)
			{
				elements[j].TransformResetTween();
			}
		}

		public override void TransformResize(float p_w, float p_h)
		{
			Transform transform = base.UnityObject.transform;
			transform.localScale = new Vector3(p_w * transform.localScale.x, p_h * transform.localScale.y, 1f);
		}

		public override void TransformColor(Color p_delta)
		{
			Color = _Color + p_delta;
		}

		public override void TransformColorEnd(Color p_color)
		{
			Color = p_color;
		}

		public override void TransformRotateX(float p_angle)
		{
			Vector3 localEulerAngles = _CachedTransform.localEulerAngles;
			localEulerAngles.x += p_angle;
			_CachedTransform.localEulerAngles = localEulerAngles;
		}

		public override void TransformRotateY(float p_angle)
		{
			Vector3 localEulerAngles = _CachedTransform.localEulerAngles;
			localEulerAngles.y += p_angle;
			_CachedTransform.localEulerAngles = localEulerAngles;
		}

		public override void TransformRotateZ(float p_angle)
		{
			Vector3 localEulerAngles = _CachedTransform.localEulerAngles;
			localEulerAngles.z += p_angle;
			_CachedTransform.localEulerAngles = localEulerAngles;
		}

		public override void TransformExecute()
		{
			for (int i = 0; i < _Element.Animations.Count; i++)
			{
				_Element.Animations[i].PlayAnimation();
			}
		}

		public override void TransformLayer(string p_layer)
		{
			for (int i = 0; i < _Element.Visuals.Count; i++)
			{
				_Element.Visuals[i].TransformLayer(p_layer);
			}
			for (int j = 0; j < _Element.UnityModels.Count; j++)
			{
				_Element.UnityModels[j].TransformLayer(p_layer);
			}
			for (int k = 0; k < Childs.Count; k++)
			{
				Childs[k].TransformLayer(p_layer);
			}
		}
	}
}
