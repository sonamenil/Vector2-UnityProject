using System;
using System.Collections;
using Nekki.Vector.Core;
using Nekki.Vector.Core.Audio;
using Nekki.Vector.Core.Counter;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Common;
using Nekki.Vector.GUI.Dialogs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nekki.Vector.GUI
{
	public class LoaderScene : Scene<LoaderScene>
	{
		private static SceneKind _PrevScene = SceneKind.None;

		private static SceneKind _NextScene = SceneKind.Main;

		private static bool _StopMusic;

		[SerializeField]
		private LabelAlias _LevelLabel;

		[SerializeField]
		private LabelAlias _LoadingLabel;

		[SerializeField]
		private LabelAlias _TipsLabel;

		[SerializeField]
		private GameObject _GradientBottom;

		[SerializeField]
		private GameObject _GradientTop;

		[SerializeField]
		private GameObject _LogoCenter;

		[SerializeField]
		private SceneBackground _Background;

		public static SceneKind PrevScene
		{
			get
			{
				return _PrevScene;
			}
			set
			{
				_PrevScene = value;
			}
		}

		public static SceneKind NextScene
		{
			set
			{
				_NextScene = value;
			}
		}

		public static bool StopMusic
		{
			set
			{
				_StopMusic = value;
			}
		}

		public override SceneKind SceneId
		{
			get
			{
				return SceneKind.Loader;
			}
		}

		protected override void Init()
		{
			base.Init();
			DialogNotificationManager.StopQueue();
			BaseDialog.OpenedDialogsCount = 0;
			if (_StopMusic)
			{
				AudioManager.StopMusic(0f);
				AudioManager.StopCutscene(0f);
				AudioManager.StopAmbient(0f);
			}
			if (LinkEditor.IsFileNew())
			{
				CounterController.Current.CounterTutorialBasicRemove();
				if (LinkEditor.TryLaunchEditorRoom())
				{
					return;
				}
			}
			if ((int)CounterController.Current.CounterTutorialBasic == 1)
			{
				RunMainController.RunStart();
				_NextScene = SceneKind.Run;
			}
			if (IsLogoLoad())
			{
				_GradientBottom.SetActive(false);
				_GradientTop.SetActive(false);
				_Background.gameObject.SetActive(false);
				_TipsLabel.gameObject.SetActive(false);
			}
			else
			{
				_LogoCenter.SetActive(false);
				_Background.AtlasName = Manager.ZoneLoaderBackground;
			}
			Resources.UnloadUnusedAssets();
			GC.Collect();
			StartCoroutine(LoadLevel());
		}

		private IEnumerator LoadLevel()
		{
			if (_PrevScene == SceneKind.None)
			{
				DebugUtils.SceneLoadStartTimer();
			}
			SetText();
			yield return new WaitForEndOfFrame();
			if (GameRestorer.Active && _PrevScene == SceneKind.None)
			{
				_NextScene = GameRestorer.RestoreScene;
			}
			Manager.Scene = _NextScene;
			LocalizationManager.SetDelayLanguage();
			yield return SceneManager.LoadSceneAsync((int)_NextScene);
		}

		public bool IsLogoLoad()
		{
			return /*_NextScene == SceneKind.Main &&*/ (int)CounterController.Current.CounterFloor <= 0;
		}

		public bool IsRunLoad()
		{
			return _NextScene == SceneKind.Run && _PrevScene != _NextScene;
		}

		public string RandomTipAlias()
		{
			int result = 0;
			int.TryParse(BalanceManager.Current.GetBalance("Tips", "Count"), out result);
			int num = UnityEngine.Random.Range(1, result + 1);
			if (DataLocal.Current.IsPaidVersion)
			{
				int result2 = 0;
				while (result2 == 0)
				{
					num = UnityEngine.Random.Range(1, result + 1);
					int.TryParse(BalanceManager.Current.GetBalance("Tips", "TipsInPaidVersion", num.ToString()), out result2);
				}
			}
			return string.Format("^GUI.Labels.Tips.{0}^", num);
		}

		private void SetText()
		{
			if (_NextScene == SceneKind.Run)
			{
				_LoadingLabel.SetAlias("^GUI.Labels.Generating^");
				_LevelLabel.SetAlias("^GUI.Labels.Level^ " + ((int)CounterController.Current.CounterFloor + 1));
			}
			_TipsLabel.SetAlias(RandomTipAlias());
		}
	}
}
