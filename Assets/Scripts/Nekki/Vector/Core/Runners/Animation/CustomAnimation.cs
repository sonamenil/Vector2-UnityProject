using System.Xml;

namespace Nekki.Vector.Core.Runners.Animation
{
	public class CustomAnimation : VisualRunner
	{
		private float _Fps;

		private int _Iteration;

		public CustomAnimation(string p_name, float p_x, float p_y, float p_width, float p_height, Element p_elements, XmlNode p_node)
			: base(p_name, p_x, p_y, p_width, p_height, p_elements, p_node)
		{
			_TypeClass = TypeRunner.CustomAnimation;
			_Fps = XmlUtils.ParseFloat(p_node.Attributes["Speed"], 10f);
			_Iteration = XmlUtils.ParseInt(p_node.Attributes["Iterations"], 1);
		}

		public override void GenerateContent()
		{
			base.GenerateContent();
			if (_CustomAnimationSprite != null)
			{
				_CustomAnimationSprite.Iterations = _Iteration;
				_CustomAnimationSprite.FPS = _Fps;
			}
		}

		public void PlayAnimation()
		{
			if (_CustomAnimationSprite != null)
			{
				_CustomAnimationSprite.Iterations = _Iteration;
				_CustomAnimationSprite.IsWork = true;
			}
		}

		public void Stop(bool Value = false)
		{
		}

		public override void TransformExecute()
		{
			PlayAnimation();
		}
	}
}
