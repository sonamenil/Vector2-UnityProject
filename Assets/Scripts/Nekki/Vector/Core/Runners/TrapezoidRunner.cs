using Nekki.Vector.Core.Game;
using System;

namespace Nekki.Vector.Core.Runners
{
    public class TrapezoidRunner : QuadRunner
    {
        private float _HeightQuad_L;

        private float _HeightQuad_R;

        public TrapezoidRunner(string p_name, int p_type, float p_x, float p_y, float p_width, float p_height, float p_height1, bool p_Stikly, Element p_elements)
            : base(p_x, p_y, p_width, p_height, p_Stikly, p_elements, p_name)
        {
            base.Type = p_type;
            _HeightQuad_L = p_height;
            _HeightQuad_R = p_height1;
            _HeightQuad = Math.Max(_HeightQuad_L, _HeightQuad_R);
        }

        public override void InitRunner()
        {
            base.InitRunner();
            Controller = UnityObject.AddComponent<TrapezoidController>();
            Controller.Base = this;
            Controller.Color = Settings.Visual.Platform.Background;
            //if (Settings.Visual.Platform.Visible)
            //    Controller.Visible = true;
        }

        protected override void CalcPoints()
        {
            _Point1.Set(_XQuad, _YQuad, 0f);
            _Point2.Set(_XQuad + _WidthQuad, _YQuad + (_HeightQuad_L - _HeightQuad_R), 0f);
            _Point3.Set(_XQuad + _WidthQuad, _YQuad + _HeightQuad_L, 0f);
            _Point4.Set(_XQuad, _YQuad + _HeightQuad_L, 0f);
        }

        protected override void SetRectangle()
        {
            Rectangle.Set(_XQuad, Math.Min(_Point1.Y, _Point2.Y), _WidthQuad, Math.Max(_HeightQuad_L, _HeightQuad_R));
        }

        public override Point GetSize(int p_sign)
        {
            return (p_sign <= 0) ? new Point(_WidthQuad, _HeightQuad_R) : new Point(_WidthQuad, _HeightQuad_L);
        }

        public override Point GetSize()
        {
            return new Point(_WidthQuad, _HeightQuad_L - _HeightQuad_R);
        }

        public override bool Hit(Vector3f p_position, bool p_equality)
        {
            Vector3f vector3f = new Vector3f(p_position);
            Vector3f vector3f2 = new Vector3f(vector3f.X - _Point4.X, vector3f.Y - _Point4.Y, 0f);
            Vector3f vector3f3 = new Vector3f(_Point4.Y - _Point3.Y, _Point3.X - _Point4.X, 0f);
            Vector3f vector3f4 = new Vector3f(vector3f.X - _Point1.X, vector3f.Y - _Point1.Y, 0f);
            Vector3f vector3f5 = new Vector3f(_Point1.Y - _Point2.Y, _Point2.X - _Point1.X, 0f);
            Vector3f vector3f6 = new Vector3f(vector3f.X - _Point3.X, vector3f.Y - _Point3.Y, 0f);
            Vector3f vector3f7 = new Vector3f(_Point3.Y - _Point2.Y, _Point2.X - _Point3.X, 0f);
            Vector3f vector3f8 = new Vector3f(vector3f.X - _Point4.X, vector3f.Y - _Point4.Y, 0f);
            Vector3f vector3f9 = new Vector3f(_Point4.Y - _Point1.Y, _Point1.X - _Point4.X, 0f);
            double num = vector3f2 * vector3f3;
            double num2 = vector3f4 * vector3f5;
            double num3 = vector3f6 * vector3f7;
            double num4 = vector3f8 * vector3f9;
            double num5 = num * num2;
            double num6 = num3 * num4;
            return p_equality ? (num5 <= 0.0 && num6 <= 0.0) : (num5 < 0.0 && num6 < 0.0);
        }

        public override void TransformResize(float p_w, float p_h)
        {
        }
    }
}
