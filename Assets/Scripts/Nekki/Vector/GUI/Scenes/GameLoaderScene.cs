using System.Collections;
using Nekki.Vector.Core;
using Nekki.Vector.Core.AssetBundle;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.GameCenter;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Dialogs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Nekki.Vector.GUI.Scenes
{
	public class GameLoaderScene : Scene<GameLoaderScene>
	{
		[SerializeField]
		private Text _LoadingLabel;

		private bool _IsInitialBundleRequestDialogShowed;

		[SerializeField]
		private VideoPlayer _VideoPlayer;

		[SerializeField]
		private RawImage _RenderTexture;

		public override SceneKind SceneId
		{
			get
			{
				return SceneKind.GameLoader;
			}
		}

		protected override void Init()
		{
			base.Init();
			_LoadingLabel.gameObject.SetActive(false);
			Application.runInBackground = Settings.PlayInBackground;
			Preloader.Instance.Init();
		}

		public void PreloaderServerActionsDone()
		{
			if ((int)CounterController.Current.CounterInitialBundleRequest > 0)
			{
				_IsInitialBundleRequestDialogShowed = true;
				if (!Settings.IsAssetBundleOn)
				{
					CounterController.Current.CounterInitialBundleRequest = 0;
					DataLocal.Current.Save(true);
					CheckBundleRequests();
				}
				else
				{
					DialogNotificationManager.ShowInitialBundleRequestDialog(0);
				}
			}
			else
			{
				CheckBundleRequests();
			}
		}

		public void CheckBundleRequests()
		{
			BundleDownloadDialogContent.OnBundleRequestsDone += BundleRequestsDone;
			if (Settings.IsAssetBundleOn && BundleManager.IsUpdateAvailable)
			{
				bool isRequiredUpdateAvailable = BundleManager.IsRequiredUpdateAvailable;
				if (_IsInitialBundleRequestDialogShowed && !isRequiredUpdateAvailable)
				{
					DialogNotificationManager.ShowBundleDownloadDialog(BundleManager.RequestsTotalContentLengthInMb);
				}
				else
				{
					DialogNotificationManager.ShowBundleRequestDialog(isRequiredUpdateAvailable, BundleManager.RequestsTotalContentLengthInMb);
				}
			}
			else
			{
				BundleRequestsDone();
			}
		}

		private void BundleRequestsDone()
		{
			BundleDownloadDialogContent.OnBundleRequestsDone -= BundleRequestsDone;
			Preloader.Instance.PostInit();
		}

		public void PreloaderWorkDone()
		{
			Scene<GameLoaderScene>.Current.StartCoroutine(Scene<GameLoaderScene>.Current.PlayIntroAndLoadGame());
		}

		public void RefreshLoadingLabel(int p_current, int p_total)
		{
			if (p_total != 0)
			{
				_LoadingLabel.gameObject.SetActive(true);
				_LoadingLabel.text = string.Format("UPDATING RESOURCES <color=#d93e21ff>{0}/{1}</color>", p_current, p_total);
			}
		}

		private IEnumerator PlayIntroAndLoadGame()
		{
			_LoadingLabel.gameObject.SetActive(false);
			if (!LinkEditor.IsFileNew())
			{
                yield return StartCoroutine(PlayVideo());
                yield return new WaitForEndOfFrame();
            }
			if (CheckPermishen())
			{
				ContinueLoadLevel();
			}
		}

		private void ContinueLoadLevel()
		{
			if (DataLocal.Current.Settings.GameCenterOn || GameCenterController.IsAutoSignInOnLaunch())
			{
				GameCenterController.SignIn();
			}
			SceneManager.LoadScene(1);
		}

		private IEnumerator PlayVideo()
		{
			_VideoPlayer.gameObject.SetActive(true);
			_RenderTexture.gameObject.SetActive(true);
			_VideoPlayer.Prepare();
			while (!_VideoPlayer.isPrepared)
			{
				yield return null;
			}
			_VideoPlayer.Play();
			while (_VideoPlayer.isPlaying)
			{
				yield return null;
			}
			yield break;
		}

		private bool CheckPermishen()
		{
			PermissionsChecker.DialogInfo p_dialogInfo = new PermissionsChecker.DialogInfo("Permishen.Title", "Permishen.TextPhoneStateStartApp");
			return PermissionsChecker.CheckPermission("android.permission.READ_PHONE_STATE", p_dialogInfo, OnGranded, OnDenied, OnUserSkip);
		}

		private void OnGranded(string p_permishen)
		{
			ContinueLoadLevel();
		}

		private void OnDenied(string p_permishen)
		{
			ContinueLoadLevel();
		}

		private void OnUserSkip(string p_permishen)
		{
			ContinueLoadLevel();
		}
	}
}
