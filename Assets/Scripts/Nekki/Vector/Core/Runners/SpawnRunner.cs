using System.Collections.Generic;
using System.Xml;
using Nekki.Vector.Core.Animation;
using Nekki.Vector.Core.Camera;
using Nekki.Vector.Core.Game;

namespace Nekki.Vector.Core.Runners
{
	public class SpawnRunner : Runner
	{
		private AnimationReaction _Reaction;

		private float _Zoom = 0.5f;

		public override bool IsDebug
		{
			get
			{
				return _IsDebug;
			}
			set
			{
				if (Settings.Visual.Spawn.Visible)
				{
					_IsDebug = value;
				}
			}
		}

		public AnimationReaction Reaction
		{
			get
			{
				return _Reaction;
			}
		}

		public float Zoom
		{
			get
			{
				return _Zoom;
			}
		}

		public SpawnRunner(Element p_elements, XmlNode p_node)
			: base(XmlUtils.ParseFloat(p_node.Attributes["X"]), XmlUtils.ParseFloat(p_node.Attributes["Y"]), p_elements)
		{
			_TypeClass = TypeRunner.Spawn;
			_Name = XmlUtils.ParseString(p_node.Attributes["Name"]);
			if (p_node.Attributes["Animation"] != null)
			{
				_Reaction = AnimationLoader.ParseReaction(new List<string>(p_node.Attributes["Animation"].Value.Split('|')));
			}
			if (p_node.Attributes["Zoom"] != null)
			{
				_Zoom = ((p_node.Attributes["Zoom"] == null) ? Nekki.Vector.Core.Camera.Camera.CurrentZoom : XmlUtils.ParseFloat(p_node.Attributes["Zoom"], Nekki.Vector.Core.Camera.Camera.CurrentZoom));
			}
		}

		public override bool Render()
		{
			return true;
		}
	}
}
