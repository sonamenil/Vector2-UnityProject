using System;
using System.Collections;
using System.Collections.Generic;
using Nekki.Vector.Core;
using Nekki.Vector.Core.AssetBundle;
using Nekki.Vector.Core.Localization;
using UIFigures;
using UnityEngine;

namespace Nekki.Vector.GUI.Dialogs
{
	public class BundleDownloadDialogContent : DialogContent
	{
		[SerializeField]
		private LabelAlias _Title;

		[SerializeField]
		private LabelAlias _Requests;

		[SerializeField]
		private UIArcBorder _ProgressCircle;

		[SerializeField]
		private LabelAlias _ProgressText;

		private int _CurrentRequest;

		private int _TotalRequests;

		private static BundleDownloadDialogContent _Current;

		public static BundleDownloadDialogContent Current
		{
			get
			{
				return _Current;
			}
		}

		public static event Action OnBundleRequestsDone;

		static BundleDownloadDialogContent()
		{
			BundleDownloadDialogContent.OnBundleRequestsDone = delegate
			{
			};
		}

		public void Init()
		{
			_Current = this;
			List<DialogButtonData> p_buttons = new List<DialogButtonData>();
			Init(p_buttons);
			BundleManager.OnBundleDownloadStarted = (Action<string>)Delegate.Combine(BundleManager.OnBundleDownloadStarted, new Action<string>(OnBundleDownloadStarted));
			BundleManager.OnBundleDownloadProgress = (Action<string, float>)Delegate.Combine(BundleManager.OnBundleDownloadProgress, new Action<string, float>(OnBundleDownloadProgress));
			BundleManager.OnBundleDownloadFinished = (Action<string, bool>)Delegate.Combine(BundleManager.OnBundleDownloadFinished, new Action<string, bool>(OnBundleDownloadFinished));
			_CurrentRequest = 0;
			_TotalRequests = BundleManager.RequestsCount;
			UpdateRequestsLabel();
			UpdateProgress(0f);
			RunBundleRequest();
		}

		public void Close()
		{
			BundleManager.OnBundleDownloadStarted = (Action<string>)Delegate.Remove(BundleManager.OnBundleDownloadStarted, new Action<string>(OnBundleDownloadStarted));
			BundleManager.OnBundleDownloadProgress = (Action<string, float>)Delegate.Remove(BundleManager.OnBundleDownloadProgress, new Action<string, float>(OnBundleDownloadProgress));
			BundleManager.OnBundleDownloadFinished = (Action<string, bool>)Delegate.Remove(BundleManager.OnBundleDownloadFinished, new Action<string, bool>(OnBundleDownloadFinished));
			_Current = null;
			base.Parent.Dismiss();
			SetBundleRequestsDone();
		}

		public void RunBundleRequest()
		{
			BundleManager.RunRequest();
		}

		private void OnBundleDownloadStarted(string p_bundleId)
		{
			UpdateProgress(0f);
		}

		private void OnBundleDownloadProgress(string p_bundleId, float p_progress)
		{
			UpdateProgress(p_progress);
		}

		private void OnBundleDownloadFinished(string p_bundleId, bool p_success)
		{
			if (!p_success)
			{
				DialogNotificationManager.ShowBundleDownloadRetryDialog(base.Parent, BundleManager.IsRequiredUpdateAvailable);
				return;
			}
			UpdateProgress(1f);
			_CurrentRequest++;
			UpdateRequestsLabel();
			if (BundleManager.RequestsCount > 0)
			{
				BundleManager.RunRequest();
			}
			else
			{
				Close();
			}
		}

		private void UpdateProgress(float p_progress)
		{
			_ProgressCircle.To = Mathf.Lerp(0f, (float)Math.PI * 2f, p_progress);
			_ProgressCircle.Refresh();
			_ProgressText.SetAlias(string.Format("{0}", (int)(p_progress * 100f)));
		}

		private void UpdateRequestsLabel()
		{
			_Requests.SetAlias(string.Format("{0}/{1}", _CurrentRequest, _TotalRequests));
		}

		public static void SetBundleRequestsDone()
		{
			ApplicationController.Current.StartCoroutine(WaitAndSetBundleRequestsDone());
		}

		private static IEnumerator WaitAndSetBundleRequestsDone()
		{
			yield return null;
			BundleDownloadDialogContent.OnBundleRequestsDone();
		}
	}
}
