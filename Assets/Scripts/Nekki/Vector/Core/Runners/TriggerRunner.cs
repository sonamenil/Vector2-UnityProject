using System;
using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Camera;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.Models;
using Nekki.Vector.Core.Node;
using Nekki.Vector.Core.Scripts;
using Nekki.Vector.Core.Trigger;
using Nekki.Vector.Core.Trigger.Events;
using Nekki.Vector.Core.Variables;
using UnityEngine;

namespace Nekki.Vector.Core.Runners
{
    public class TriggerRunner : QuadRunner, IVariableParent
    {
        public enum TriggerColisionType
        {
            OneNode = 0,
            MultiNode = 1
        }

        private enum TriggerType
        {
            TT_Rectangle = 0,
            TT_Elips = 1,
            TT_UpDiagonal = 2,
            TT_DownDiagonal = 3,
            TT_Circle = 4
        }

        private TriggerTimer _Timer;

        private Variable _AIvar;

        private Variable _NodeVar;

        private Variable _ModelVar;

        private Variable _ActiveVar;

        private ModelHuman _CheckedModel;

        private TriggerColisionType _CollisionType;

        private TriggerType _TriggerType;

        private Dictionary<string, Variable> _Vars = new Dictionary<string, Variable>();

        private List<TriggerRunnerLoop> _Loops = new List<TriggerRunnerLoop>();

        private List<TriggerLine> _Lines;

        private List<TRE_ChangeVar> _RenderEvents = new List<TRE_ChangeVar>();

        private string _CollisionNodeName;

        private XmlNode _Node;

        private TriggerCameraDetector _Detector;

        private TriggerCameraDetectorWidescreen _DetectorWS;

        public List<string> _NodesName;

        public string CollisionNodeName
        {
            get
            {
                return _CollisionNodeName;
            }
            set
            {
                _CollisionNodeName = value;
            }
        }

        public override bool IsCollisible
        {
            get
            {
                for (int i = 0; i < _Loops.Count; i++)
                {
                    if (_Loops[i].IsContainsCollisionEvent)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override bool IsEnabled
        {
            get
            {
                return base.IsEnabled;
            }
        }

        public TriggerCameraDetector Detector
        {
            get
            {
                return _Detector;
            }
        }

        public TriggerCameraDetectorWidescreen DetectorWS
        {
            get
            {
                return _DetectorWS;
            }
        }

        public Variable AIVar
        {
            get
            {
                return _AIvar;
            }
        }

        public Variable ModelVar
        {
            get
            {
                return _ModelVar;
            }
        }

        public string TriggerNodeName
        {
            get
            {
                if (_NodeVar != null)
                {
                    return _NodeVar.ValueString;
                }
                return "COM";
            }
        }

        public List<string> TriggerNodesName
        {
            get
            {
                return _NodesName;
            }
        }

        public bool IsActive
        {
            get
            {
                return _ActiveVar.ValueInt == 1;
            }
        }

        public List<TriggerLine> Lines
        {
            get
            {
                return _Lines;
            }
        }

        public TriggerColisionType CollisionType
        {
            get
            {
                return _CollisionType;
            }
        }

        public bool IsRectType
        {
            get
            {
                return _TriggerType == TriggerType.TT_Rectangle;
            }
        }

        public bool IsElipsType
        {
            get
            {
                return _TriggerType == TriggerType.TT_Elips;
            }
        }

        public bool IsUpDiagonal
        {
            get
            {
                return _TriggerType == TriggerType.TT_UpDiagonal;
            }
        }

        public bool IsDownDiagonal
        {
            get
            {
                return _TriggerType == TriggerType.TT_DownDiagonal;
            }
        }

        public bool IsDiagonal
        {
            get
            {
                return IsUpDiagonal || IsDownDiagonal;
            }
        }

        public bool IsCircleType
        {
            get
            {
                return _TriggerType == TriggerType.TT_Circle;
            }
        }

        public Vector3 Center
        {
            get
            {
                return base.Position + new Vector3(_WidthQuad * 0.5f, _HeightQuad * 0.5f, 0f);
            }
        }

        public TriggerRunner(float p_x, float p_y, float p_width, float p_height, Element p_elements, XmlNode p_node)
            : base(p_x, p_y, p_width, p_height, false, p_elements, XmlUtils.ParseString(p_node.Attributes["Name"], string.Empty))
        {
            _TypeClass = TypeRunner.Trigger;
            _Timer = new TriggerTimer(this);
            _CollisionType = TriggerColisionType.OneNode;
            _TriggerType = GetTriggerType(XmlUtils.ParseString(p_node.Attributes["Type"], "Rectangle"));
            if (_TriggerType == TriggerType.TT_Circle && (double)Math.Abs(p_width - p_height) > 0.1)
            {
                DebugUtils.Dialog("WARNING: detected trigger with type 'circle' and Height!=Width. Name = " + base.Name, false);
                _TriggerType = TriggerType.TT_Elips;
            }
            _Node = p_node["Content"];
        }

        public override void InitRunner()
        {
            base.InitRunner();
            Controller = UnityObject.AddComponent<TriggerController>();
            Controller.Base = this;
            Controller.Color = Settings.Visual.Trigger.Background;
            //if (Settings.Visual.Trigger.Visible)
            //    Controller.Visible = true;
        }

        public void Init()
        {
            ParseVariable(_Node["Init"]);
            SetTriggerCollisionType();
            XmlNode xmlNode = _Node["Template"];
            if (xmlNode != null)
            {
                string value = xmlNode.Attributes["Name"].Value;
                ParseTemplate(TemplateModule.getTemplateXmlNode(value));
            }
            ParseLoops(_Node);
            InitRenderEvents();
            _Node = null;
        }

        public override void SetEnabled(bool p_enabled, bool restore = false, bool fromHierarchy = false)
        {
            base.SetEnabled(p_enabled, restore, fromHierarchy);
            if (p_enabled)
            {
                return;
            }
            List<ModelHuman> models = RunMainController.Models;
            foreach (ModelHuman item in models)
            {
                item.ControllerTrigger.RemoveTrigger(this);
            }
        }

        private void SetTriggerCollisionType()
        {
            string[] array = _NodeVar.ValueString.Split('|');
            if (array.Length == 1)
            {
                _CollisionType = TriggerColisionType.OneNode;
                _CollisionNodeName = array[0];
            }
            else
            {
                _CollisionType = TriggerColisionType.MultiNode;
                _NodesName = new List<string>(array);
            }
        }

        private TriggerType GetTriggerType(string p_value)
        {
            switch (p_value)
            {
                case "Rectangle":
                    return TriggerType.TT_Rectangle;
                case "Ellipse":
                    return TriggerType.TT_Elips;
                case "UpDiagonal":
                    return TriggerType.TT_UpDiagonal;
                case "DownDiagonal":
                    return TriggerType.TT_DownDiagonal;
                case "Circle":
                    return TriggerType.TT_Circle;
                default:
                    return TriggerType.TT_Rectangle;
            }
        }

        public void CreateCameraDetector()
        {
            if (!(_Detector != null))
            {
                _Detector = _UnityObject.AddComponent<TriggerCameraDetector>();
                _Detector.Base = this;
            }
        }

        public void CreateCameraDetectorWidescreen()
        {
            if (_DetectorWS == null)
            {
                _DetectorWS = new TriggerCameraDetectorWidescreen();
                _DetectorWS.Base = this;
                Nekki.Vector.Core.Camera.Camera.Current.WidescreenCameraDetectors.Add(_DetectorWS);
            }
        }

        private void InitRenderEvents()
        {
            foreach (TriggerRunnerLoop loop in _Loops)
            {
                foreach (TriggerEvent @event in loop.Events)
                {
                    if (@event.Type == TriggerEvent.EventType.TRE_VAR_CHANGE)
                    {
                        _RenderEvents.Add((TRE_ChangeVar)@event);
                    }
                }
            }
        }

        public void ParseTemplate(XmlNode p_node)
        {
            ParseLoops(p_node);
        }

        public void ParseVariable(XmlNode p_node, bool p_isTemplate = false)
        {
            if (p_node == null)
            {
                return;
            }
            foreach (XmlNode childNode in p_node.ChildNodes)
            {
                if (!childNode.LocalName.Equals("SetVariable"))
                {
                    continue;
                }
                string text = XmlUtils.ParseString(childNode.Attributes["Name"]);
                string text2 = XmlUtils.ParseString(childNode.Attributes["Value"], string.Empty);
                if (_Vars.ContainsKey("_" + text))
                {
                    switch (_Vars["_" + text].Type)
                    {
                        case VariableType.Int:
                            _Vars["_" + text].SetValue(int.Parse(text2.ToString()));
                            break;
                        case VariableType.Float:
                            _Vars["_" + text].SetValue(float.Parse(text2));
                            break;
                        case VariableType.String:
                            _Vars["_" + text].SetValue(text2);
                            break;
                    }
                }
                else
                {
                    _Vars["_" + text] = Variable.CreateVariable(text2, text, this);
                }
            }
            if (!p_isTemplate)
            {
                _AIvar = _Vars["_$AI"];
                _NodeVar = _Vars["_$Node"];
                _ActiveVar = _Vars["_$Active"];
                _Vars["_$ActionID"] = Variable.CreateVariable(" ", "$ActionID", this);
                _Vars["_$WaypointKey"] = Variable.CreateVariable(" ", "$WaypointKey", this);
                if (!_Vars.ContainsKey("_$Model"))
                {
                    _ModelVar = Variable.CreateVariable(" ", "$Model", this);
                    _Vars["_$Model"] = _ModelVar;
                }
                else
                {
                    _ModelVar = _Vars["_$Model"];
                }
                _Vars["_$Key"] = Variable.CreateVariable(" ", "$Key", this);
                if (_AIvar == null)
                {
                    _AIvar = Variable.CreateVariable("-1", "$AI");
                }
                if (_ActiveVar == null)
                {
                }
                if (_NodeVar != null)
                {
                }
            }
        }

        public void ParseLoops(XmlNode p_node)
        {
            foreach (XmlNode childNode in p_node.ChildNodes)
            {
                if (string.Equals(childNode.LocalName, "Loop"))
                {
                    TriggerRunnerLoop triggerRunnerLoop;
                    if (childNode.Attributes["Template"] != null)
                    {
                        string value = childNode.Attributes["Template"].Value;
                        XmlNode templateLoopXML = TemplateModule.getTemplateLoopXML(value);
                        triggerRunnerLoop = TriggerRunnerLoop.Create(templateLoopXML, this);
                        _Loops.Add(triggerRunnerLoop);
                    }
                    else
                    {
                        triggerRunnerLoop = TriggerRunnerLoop.Create(childNode, this);
                        _Loops.Add(triggerRunnerLoop);
                    }
                    triggerRunnerLoop.Number = _Loops.Count;
                }
            }
        }

        public void CheckEvent(TriggerRunnerEvent p_event, ModelHuman p_model)
        {
            if (!IsEnabled || (!IsActive && !p_event.IsTimeOutOrActivateOrOnStartGame()) || (p_model != null && p_model.IsPhysics && !p_event.IsCollision()))
            {
                return;
            }
            VectorLog.RunLog(p_event, base.Name, base.Position, true);
            _CheckedModel = p_model;
            VectorLog.Tab(1);
            List<List<TriggerRunnerAction>> list = new List<List<TriggerRunnerAction>>();
            for (int i = 0; i < _Loops.Count; i++)
            {
                _Loops[i].ProcessEvent(p_event, list);
            }
            VectorLog.Untab(1);
            if (list.Count != 0)
            {
                VectorLog.RunLog(p_event, base.Name, base.Position, false);
                VectorLog.Tab(1);
                for (int j = 0; j < list.Count; j++)
                {
                    VectorLog.RunLog("Loop: " + (j + 1));
                    VectorLog.Tab(1);
                    RenderTriggerActions.Current.AddActions(list[j]);
                    VectorLog.Untab(1);
                }
                VectorLog.Untab(1);
                list.Clear();
                _ModelVar.SetValue(string.Empty);
                _CheckedModel = null;
            }
        }

        public void ResetRenderEvents()
        {
            int count = _RenderEvents.Count;
            for (int i = 0; i < count; i++)
            {
                _RenderEvents[i].Reset();
            }
        }

        public void CheckRenderEvent(ModelHuman p_model)
        {
            if (_RenderEvents == null)
            {
                return;
            }
            _ModelVar.SetValue(p_model.ModelName);
            foreach (TRE_ChangeVar renderEvent in _RenderEvents)
            {
                if (renderEvent.IsChange())
                {
                    CheckEvent(renderEvent, p_model);
                    _ModelVar.SetValue(p_model.ModelName);
                }
            }
            _ModelVar.SetValue(string.Empty);
        }

        public override bool Render()
        {
            return _Timer.Render();
        }

        public Variable GetVariable(string p_key)
        {
            if (!_Vars.ContainsKey(p_key))
            {
                DebugUtils.Dialog("No Var Name = " + p_key + " in trigger " + base.Name, true);
                return null;
            }
            return _Vars[p_key];
        }

        public void ChangeVarByName(string p_name, Variable p_var)
        {
            _Vars["_" + p_name] = p_var;
        }

        public SpawnRunner GetSpawnByName(string p_name)
        {
            foreach (SpawnRunner spawn in ParentElements.Spawns)
            {
                if (spawn.Name == p_name)
                {
                    return spawn;
                }
            }
            return null;
        }

        public void SetModelVar()
        {
            if (_CheckedModel != null)
            {
                ModelVar.SetValue(_CheckedModel.ModelName);
            }
        }

        public void AddLine(TriggerLine p_line)
        {
            if (_Lines == null)
            {
                _Lines = new List<TriggerLine>();
            }
            _Lines.Add(p_line);
        }

        public void SetKeyVar(string p_key)
        {
            _Vars["_$Key"].SetValue(p_key);
        }

        public void SetTimer(int p_frames)
        {
            _Timer.Start(p_frames);
        }

        public bool Hit(ModelNode p_node, bool Equality = false)
        {
            switch (_TriggerType)
            {
                case TriggerType.TT_Elips:
                    return HitElips(p_node.Start);
                case TriggerType.TT_Rectangle:
                    return base.Hit(p_node.Start, Equality);
                case TriggerType.TT_UpDiagonal:
                    return HitUpDiagonal(p_node);
                case TriggerType.TT_DownDiagonal:
                    return HitDownDiagonal(p_node);
                case TriggerType.TT_Circle:
                    return HitCircle(p_node.Start);
                default:
                    return false;
            }
        }

        public bool HitElips(Vector3f p_point)
        {
            bool flag = Rectangle.Size.Width >= Rectangle.Size.Height;
            float num = ((!flag) ? (Rectangle.Size.Height / 2f) : (Rectangle.Size.Width / 2f));
            float num2 = ((!flag) ? (Rectangle.Size.Width / 2f) : (Rectangle.Size.Height / 2f));
            float num3 = Mathf.Sqrt(1f - num2 * num2 / (num * num));
            float num4 = num * num3;
            float midX = Rectangle.MidX;
            float midY = Rectangle.MidY;
            Vector2 vector = ((!flag) ? new Vector2(midX, midY - num4) : new Vector2(midX - num4, midY));
            Vector2 vector2 = ((!flag) ? new Vector2(midX, midY + num4) : new Vector2(midX + num4, midY));
            float num5 = Mathf.Sqrt(Mathf.Pow(vector.x - p_point.X, 2f) + Mathf.Pow(vector.y - p_point.Y, 2f));
            float num6 = Mathf.Sqrt(Mathf.Pow(vector2.x - p_point.X, 2f) + Mathf.Pow(vector2.y - p_point.Y, 2f));
            if (num5 + num6 < 2f * num)
            {
                return true;
            }
            return false;
        }

        private bool HitUpDiagonal(ModelNode p_node)
        {
            return p_node.CroosLine(Rectangle.BottomLeft, Rectangle.TopRight);
        }

        private bool HitDownDiagonal(ModelNode p_node)
        {
            return p_node.CroosLine(Rectangle.TopLeft, Rectangle.BottomRight);
        }

        private bool HitCircle(Vector3f p_point)
        {
            float num = Mathf.Sqrt(Mathf.Pow(Rectangle.MidX - p_point.X, 2f) + Mathf.Pow(Rectangle.MidY - p_point.Y, 2f));
            return num < Rectangle.Size.Width / 2f;
        }

        public override void End()
        {
            base.End();
            if (_Detector != null)
            {
                _Detector.End();
                _Detector = null;
            }
            if (_DetectorWS != null)
            {
                _DetectorWS.End();
                _DetectorWS = null;
            }
            _UnityObject = null;
        }

        public override string ToString()
        {
            string text = "Trigger: " + base.Name + "\nParentObject Name: " + ParentElements.Parent.Name + "\nVars:";
            text = text + "$AI:" + _AIvar.ValueInt;
            text = text + " $Active:" + _ActiveVar.ValueInt;
            text = text + " $Node:" + _NodeVar.ValueString;
            text = text + " $ActionID:" + _Vars["_$ActionID"].ValueString;
            foreach (Variable value in _Vars.Values)
            {
                if (!value.Name.Contains("$"))
                {
                    text = text + "\n " + value.ToString();
                }
            }
            string text2 = text;
            text = text2 + "\nLoops (" + _Loops.Count + "):";
            foreach (TriggerRunnerLoop loop in _Loops)
            {
                text = text + "\n " + loop.ToString();
                text += "\n--------------------";
            }
            return text;
        }
    }
}
