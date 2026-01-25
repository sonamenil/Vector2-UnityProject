using System.Xml;
using Nekki.Vector.Core.AssetBundle;
using Nekki.Vector.Core.Variables;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TQA_AddBundleRequest : TriggerQuestAction
	{
		private Variable _BundleId;

		public TQA_AddBundleRequest(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_parent)
		{
			_BundleId = Variable.CreateVariable(XmlUtils.ParseString(p_node.Attributes["BundleId"]), string.Empty);
		}

		public override void Activate(ref bool p_runNext)
		{
			p_runNext = true;
			string valueString = _BundleId.ValueString;
			if (!BundleManager.IsBundleExists(valueString))
			{
				BundleManager.CreateBundleRequestWithCheckingUpdate(valueString, 0, true);
			}
		}
	}
}
