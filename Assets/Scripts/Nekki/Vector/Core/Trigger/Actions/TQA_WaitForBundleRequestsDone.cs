using System.Xml;
using Nekki.Vector.Core.AssetBundle;
using Nekki.Vector.Core.Game;
using Nekki.Vector.GUI.Dialogs;

namespace Nekki.Vector.Core.Trigger.Actions
{
	public class TQA_WaitForBundleRequestsDone : TriggerQuestAction
	{
		public TQA_WaitForBundleRequestsDone(XmlNode p_node, TriggerQuestLoop p_parent)
			: base(p_parent)
		{
		}

		public override void Activate(ref bool p_runNext)
		{
			if (!Settings.IsAssetBundleOn || !BundleManager.IsUpdateAvailable)
			{
				p_runNext = true;
				return;
			}
			p_runNext = false;
			BundleDownloadDialogContent.OnBundleRequestsDone += BundleRequestsOver;
			DialogNotificationManager.ShowBundleRequestDialog(BundleManager.IsRequiredUpdateAvailable, BundleManager.RequestsTotalContentLengthInMb);
		}

		private void BundleRequestsOver()
		{
			BundleDownloadDialogContent.OnBundleRequestsDone -= BundleRequestsOver;
			_Parent.ActivateActions();
		}
	}
}
