using System;
using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Utilites;
using Nekki.Vector.GUI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Dialogs
{
	public class MissionsDialog : BaseDialog
	{
		[SerializeField]
		private GameObject _MissionPrefab;

		[SerializeField]
		private Image _Background;

		[SerializeField]
		private LayoutGroup _MissionsLayout;

		[SerializeField]
		private Button _BackButton;

		[SerializeField]
		private Button _ContinueButton;

		private List<MissionItemUI> _MissionsTabsList = new List<MissionItemUI>();

		private static Color _RunBgColor = ColorUtils.FromHex("0A0A0A80");

		private static MissionsDialog _Current;

		public static MissionsDialog Current
		{
			get
			{
				return _Current;
			}
		}

		public static event Action OnOpen;

		public static event Action OnClose;

		static MissionsDialog()
		{
			MissionsDialog.OnOpen = delegate
			{
			};
			MissionsDialog.OnClose = delegate
			{
			};
		}

		public void Init(bool p_isWithContinueButton)
		{
			_Current = this;
			if (Manager.IsShop)
			{
				MissionsManager.RunMissionCardsRefresher();
			}
			DestroyMissionTabs();
			CreateMissionsTabs();
			if (Manager.IsRun)
			{
				_Background.color = _RunBgColor;
			}
			_BackButton.gameObject.SetActive(!Manager.IsRun && !p_isWithContinueButton);
			_ContinueButton.gameObject.SetActive(!Manager.IsRun && p_isWithContinueButton);
			MissionsDialog.OnOpen();
		}

		public void Close()
		{
			MissionsDialog.OnClose();
			Dismiss();
			_Current = null;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			_Current = null;
		}

		private void DestroyMissionTabs()
		{
			for (int i = 0; i < _MissionsTabsList.Count; i++)
			{
				UnityEngine.Object.Destroy(_MissionsTabsList[i].gameObject);
			}
			_MissionsTabsList.Clear();
		}

		private void CreateMissionsTabs()
		{
			foreach (MissionItem missionItem in MissionsManager.MissionItems)
			{
				MissionItemUI component = UnityEngine.Object.Instantiate(_MissionPrefab).GetComponent<MissionItemUI>();
				component.transform.SetParent(_MissionsLayout.transform, false);
				component.Init(missionItem);
				_MissionsTabsList.Add(component);
			}
		}

		public void OnButtonsTap()
		{
			Close();
		}

		public void OnDialogTap()
		{
			if (!Manager.IsRun)
			{
				Close();
			}
		}
	}
}
