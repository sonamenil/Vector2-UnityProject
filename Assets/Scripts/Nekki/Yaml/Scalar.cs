using System;
using YamlDotNet.RepresentationModel;

namespace Nekki.Yaml
{
	[Serializable]
	public class Scalar : Node
	{
		public delegate void ScalarUpdateEventHandler();

		private YamlScalarNode _scalar;

		public string text
		{
			get
			{
				return _scalar.Value;
			}
		}

		public static event ScalarUpdateEventHandler TextUpdate;

		public Scalar(string keyNew, YamlScalarNode scalarNew)
		{
			base.typeNode = "Scalar";
			base.key = keyNew;
			base.value = scalarNew;
			_scalar = (YamlScalarNode)base.value;
		}

		public Scalar(string keyNew, string valueNew)
		{
			base.typeNode = "Scalar";
			base.key = keyNew;
			base.value = new YamlScalarNode(valueNew);
			_scalar = (YamlScalarNode)base.value;
		}

		private static void OnTextUpdate()
		{
			ScalarUpdateEventHandler textUpdate = Scalar.TextUpdate;
			if (textUpdate != null)
			{
				textUpdate();
			}
		}

		public void SetText(string valueNew)
		{
			_scalar.Value = valueNew;
			OnTextUpdate();
		}

		public string GetText()
		{
			return _scalar.Value;
		}
	}
}
