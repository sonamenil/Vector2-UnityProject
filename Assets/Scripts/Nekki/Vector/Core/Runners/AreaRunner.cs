using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Animation;
using Nekki.Vector.Core.Controllers;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.Models;
using UnityEngine;

namespace Nekki.Vector.Core.Runners
{
    public class AreaRunner : QuadRunner
    {
        protected string _TypeName;

        public override bool IsDebug
        {
            get
            {
                return _IsDebug;
            }
            set
            {
                if (Settings.Visual.Area.Visible)
                {
                    _IsDebug = value;
                }
            }
        }

        public override bool IsEnabled
        {
            get
            {
                return base.IsEnabled;
            }
        }

        public AreaRunner(float p_x, float p_y, float p_width, float p_height, Element p_elements, XmlNode p_node)
            : base(p_x, p_y, p_width, p_height, false, p_elements, XmlUtils.ParseString(p_node.Attributes["Name"], string.Empty))
        {
            _TypeClass = TypeRunner.Area;
            _TypeName = p_node.Attributes["Type"].Value;
            LoadBinaryIfTrick();
        }

        public override void InitRunner()
        {
            base.InitRunner();
            Controller = UnityObject.AddComponent<QuadController>();
            Controller.Base = this;
            Controller.Color = Settings.Visual.Area.Background;
            //        if (Settings.Visual.Area.Visible)
            //Controller.Visible = true;
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
                item.ControllerCollision.ControllerArea.RemoveArea(this);
            }
        }

        private void LoadBinaryIfTrick()
        {
            if (Animations.Animation.ContainsKey(_Name))
            {
                Core.Animation.AnimationInfo animationInfo = Animations.Animation[_Name];
                if (animationInfo.IsTrick)
                {
                    AnimationTrickInfo.LoadAnimation(animationInfo as AnimationTrickInfo);
                }
            }
        }

        public virtual bool OnKeyClick(KeyVariables p_key)
        {
            return true;
        }

        public virtual void Activate(ModelHuman p_model)
        {
        }

        public virtual void Deactivate(ModelHuman p_model)
        {
        }

        public override string ToString()
        {
            string text = "Area Name:" + _Name;
            string text2 = text;
            text = text2 + "\nX=" + _XQuad + " Y=" + _YQuad;
            text2 = text;
            return text2 + "\nWidth=" + _WidthQuad + " Height=" + _HeightQuad;
        }
    }
}
