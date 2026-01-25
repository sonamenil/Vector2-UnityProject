using Nekki.Vector.Core.AssetBundle;
using Nekki.Vector.Core.Audio.Internal;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.User;
using Nekki.Vector.GUI.Dialogs;
using Nekki.Vector.GUI.InputControllers;
using UnityEngine;

namespace Nekki.Vector.GUI
{
	public abstract class Scene<T> : ModuleHolder where T : Scene<T>
	{
		[SerializeField]
		private DialogCanvasController _DialogCanvasPrefab;

		[SerializeField]
		private DebugUI _DebugCanvasPrefab;

		private Canvas _Canvas;

		private KeyboardController _KeyboardController;

		protected static T _Current;

		protected bool _IsInited;

		protected bool _IsReleased;

		public Canvas Canvas
		{
			get
			{
				return _Canvas;
			}
		}

		public KeyboardController KeyboardController
		{
			get
			{
				return _KeyboardController;
			}
		}

		public static T Current
		{
			get
			{
				return _Current;
			}
		}

		public abstract SceneKind SceneId { get; }

		protected virtual void Init()
		{
			_IsInited = true;
		}

		protected virtual void Free()
		{
			_IsReleased = true;
		}

		protected override void Awake()
		{
			T val = this as T;
			if (Manager.Init(val.SceneId))
			{
				_Current = this as T;
				_Canvas = _Current.GetComponent<Canvas>();
				_KeyboardController = _Current.GetComponent<KeyboardController>();
				SetupQuality();
				MountDialogPrefab();
				MountDebugPrefab();
				base.Awake();
				Init();
			}
		}

		private void SetupQuality()
		{
			if (DataLocal.IsCurrentExists && DataLocal.Current.Settings.UseLowResGraphics)
			{
				QualitySettings.antiAliasing = 0;
				return;
			}
			switch (SceneId)
			{
			case SceneKind.GameLoader:
			case SceneKind.Loader:
			case SceneKind.Main:
			case SceneKind.Shop:
			case SceneKind.Terminal:
				QualitySettings.antiAliasing = 2;
				break;
			default:
				QualitySettings.antiAliasing = 0;
				break;
			}
		}

		private void MountDialogPrefab()
		{
			if (_DialogCanvasPrefab != null)
			{
				DialogCanvasController dialogCanvasController = Object.Instantiate(_DialogCanvasPrefab);
				dialogCanvasController.name = "[DialogCanvas]";
			}
		}

		private void MountDebugPrefab()
		{
			if (_DebugCanvasPrefab != null && !Settings.IsReleaseBuild)
			{
				DebugUI debugUI = Object.Instantiate(_DebugCanvasPrefab);
				debugUI.name = "[DebugCanvas]";
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			AtlasCache.Clear();
			AudioCache.Clear();
			BundleManager.UnloadAllAssets();
			if (!_IsReleased)
			{
				Free();
			}
			_Current = (T)null;
		}
	}
}
