using System;
using Nekki.Utils;
using UnityEngine;

namespace Nekki.Core
{
	public abstract class GameInit
	{
		public delegate void OnInitializeDoneEventHandler();

		public static event OnInitializeDoneEventHandler InitializeDone;

		private static void OnInitializeDone()
		{
			OnInitializeDoneEventHandler initializeDone = GameInit.InitializeDone;
			if (initializeDone != null)
			{
				initializeDone();
			}
		}

		protected void OnNekkiAssetDownloaderInitDone()
		{
			OnInitializeDone();
		}

		public virtual void Subscribe(params Action[] actions)
		{
			foreach (Action action in actions)
			{
				Action action2 = action;
				GameInit.InitializeDone = (OnInitializeDoneEventHandler)Delegate.Combine(GameInit.InitializeDone, (OnInitializeDoneEventHandler)delegate
				{
					action2();
				});
			}
		}

		public virtual void Initialize(bool externalInit = false)
		{
			GlobalTimer.Init(externalInit);
			Application.targetFrameRate = 60;
			NekkiAssetDownloader.Instance.Init(GlobalPaths.AssetServer, OnNekkiAssetDownloaderInitDone);
		}

		public abstract void Init(params Action[] actions);
	}
}
