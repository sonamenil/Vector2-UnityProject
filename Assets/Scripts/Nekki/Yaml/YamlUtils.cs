using UnityEngine;

namespace Nekki.Yaml
{
	public class YamlUtils
	{
		public static Vector2 YamlSequenceToVector2(Sequence seq)
		{
			Vector2 vector = default(Vector2);
			vector = Vector2.zero;
			if (seq != null)
			{
				AdvLog.Log(string.Format("<{0}>", seq.GetType()));
				vector.x = float.Parse(((Scalar)seq.nodesInside[0]).text);
				vector.y = float.Parse(((Scalar)seq.nodesInside[1]).text);
			}
			return vector;
		}
	}
}
