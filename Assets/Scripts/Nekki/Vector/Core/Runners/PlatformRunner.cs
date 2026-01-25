using Nekki.Vector.Core.Game;
using UnityEngine;

namespace Nekki.Vector.Core.Runners
{
    public class PlatformRunner : QuadRunner
    {
        public PlatformRunner(string p_name, float p_x, float p_y, float p_width, float p_height, bool p_stikly, Element p_elements)
            : base(p_x, p_y, p_width, p_height, p_stikly, p_elements, p_name)
        {
        }

        public override void InitRunner()
        {
            base.InitRunner();
            Controller = UnityObject.AddComponent<QuadController>();
            Controller.Base = this;
            Controller.Color = Settings.Visual.Platform.Background;
            //if (Settings.Visual.Platform.Visible)
            //    Controller.Visible = true;
        }
        public override string ToString()
        {
            string text = "Platform Name:" + _Name;
            text = text + "\nParentObject Name: " + ParentElements.Parent.Name;
            string text2 = text;
            text = text2 + "\nX=" + _XQuad + " Y=" + _YQuad;
            text2 = text;
            text = text2 + "\nWidth=" + _WidthQuad + " Height=" + _HeightQuad;
            text2 = text;
            text = text2 + "\nPoint1.X = " + _Point1.X + " Point1.Y=" + _Point1.Y;
            text2 = text;
            text = text2 + "\nPoint2.X = " + _Point2.X + " Point2.Y=" + _Point2.Y;
            text2 = text;
            text = text2 + "\nPoint3.X = " + _Point3.X + " Point3.Y=" + _Point3.Y;
            text2 = text;
            text = text2 + "\nPoint4.X = " + _Point4.X + " Point4.Y=" + _Point4.Y;
            text2 = text;
            return text2 + "\nSpeedRunner.X = " + base.SpeedRunner.X + " SpeedRunner.Y = " + base.SpeedRunner.Y;
        }
    }
}
