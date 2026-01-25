using System;
using System.Collections.Generic;
using Assets.src.GUI.Dialogs;
using Nekki.Vector.Core.GameManagement;
using Nekki.Vector.Core.Quest;
using Nekki.Vector.GUI.Dialogs.Payment;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

namespace Nekki.Vector.GUI.Dialogs
{
	public class DialogCanvasController : MonoBehaviour
	{
		[SerializeField]
		private RawImage _Background;

		[SerializeField]
		private GameObject _BaseUIDialogPrefab;

		[SerializeField]
		private GameObject _EndFloorDialogPrefab;

		[SerializeField]
		private GameObject _OptionsDialogPrefab;

		[SerializeField]
		private GameObject _PaymentDialogPrefab;

		[SerializeField]
		private GameObject _SaveMeContentPrefab;

		[SerializeField]
		private GameObject _EnergyDialogContentPrefab;

		[SerializeField]
		private GameObject _SelectCardContentOrefab;

		[SerializeField]
		private GameObject _SaleDialogContentPrefab;

		[SerializeField]
		private GameObject _SimpleNotificationPrefab;

		[SerializeField]
		private GameObject _MissionNotificationPrefab;

		[SerializeField]
		private GameObject _BaseTooltipPrefab;

		[SerializeField]
		private GameObject _PromoEffectDialogContentPrefab;

		[SerializeField]
		private GameObject _NewsDialogContentPrefab;

		[SerializeField]
		private GameObject _QuestStartDialogContent;

		[SerializeField]
		private GameObject _QuestCompleteDialogContent;

		[SerializeField]
		private GameObject _QuestTalkingDialogContent;

		[SerializeField]
		private GameObject _MissionsDialog;

		[SerializeField]
		private GameObject _ChapterWindow;

		[SerializeField]
		private GameObject _BundleDownloadDialogContent;

		private List<GraphicRaycaster> _GraphicRaycasters;

		private BlurOptimized _BlurEffect;

		private Camera _BlurEffectCamera;

		private RenderTexture _BlurEffectTexture;

		private Dictionary<string, GameObject> _PrefabsByType = new Dictionary<string, GameObject>();

		private Dictionary<string, BaseDialog> _CreatedDialogs = new Dictionary<string, BaseDialog>();

		private static DialogCanvasController _Current;

		private static Canvas _DialogsCanvas;

		public static DialogCanvasController Current
		{
			get
			{
				return _Current;
			}
		}

		public static Canvas DialogsCanvas
		{
			get
			{
				return _DialogsCanvas;
			}
		}

		private void Awake()
		{
			_Current = this;
			_DialogsCanvas = GetComponent<Canvas>();
			Camera[] allCameras = Camera.allCameras;
			_BlurEffectCamera = allCameras[0];
			_BlurEffect = _BlurEffectCamera.gameObject.GetComponent<BlurOptimized>();
			_Background.gameObject.SetActive(false);
			GraphicRaycaster[] collection = UnityEngine.Object.FindObjectsOfType<GraphicRaycaster>();
			_GraphicRaycasters = new List<GraphicRaycaster>(collection);
			InitPrefabsByTypeDictionary();
		}

		private void InitPrefabsByTypeDictionary()
		{
			_PrefabsByType.Add(typeof(EndFloorDialog).Name, _EndFloorDialogPrefab);
			_PrefabsByType.Add(typeof(InfoDialogContent).Name, _SaveMeContentPrefab);
			_PrefabsByType.Add(typeof(OptionsDialog).Name, _OptionsDialogPrefab);
			_PrefabsByType.Add(typeof(EnergyDialogContent).Name, _EnergyDialogContentPrefab);
			_PrefabsByType.Add(typeof(PaymentDialog).Name, _PaymentDialogPrefab);
			_PrefabsByType.Add(typeof(SelectCardDialogContent).Name, _SelectCardContentOrefab);
			_PrefabsByType.Add(typeof(SaleDialogContent).Name, _SaleDialogContentPrefab);
			_PrefabsByType.Add(typeof(SimpleNotification).Name, _SimpleNotificationPrefab);
			_PrefabsByType.Add(typeof(MissionNotification).Name, _MissionNotificationPrefab);
			_PrefabsByType.Add(typeof(Tooltip).Name, _BaseTooltipPrefab);
			_PrefabsByType.Add(typeof(PromoEffectDialogContent).Name, _PromoEffectDialogContentPrefab);
			_PrefabsByType.Add(typeof(NewsDialogContent).Name, _NewsDialogContentPrefab);
			_PrefabsByType.Add(typeof(QuestStartDialogContent).Name, _QuestStartDialogContent);
			_PrefabsByType.Add(typeof(QuestCompleteDialogContent).Name, _QuestCompleteDialogContent);
			_PrefabsByType.Add(typeof(QuestTalkingDialogContent).Name, _QuestTalkingDialogContent);
			_PrefabsByType.Add(typeof(ChapterWindow).Name, _ChapterWindow);
			_PrefabsByType.Add(typeof(MissionsDialog).Name, _MissionsDialog);
			_PrefabsByType.Add(typeof(BundleDownloadDialogContent).Name, _BundleDownloadDialogContent);
		}

		private void OnDestroy()
		{
			_Current = null;
			_DialogsCanvas = null;
			_CreatedDialogs.Clear();
		}

		public void TurnOnBlurEffect()
		{
			_BlurEffect.SetTexturAndTurnOff(_Background);
		}

		public void TurnOffBlurEffect()
		{
			_BlurEffect.Reset(_Background);
		}

		public void ReinitiateBlur()
		{
			TurnOffBlurEffect();
			TurnOnBlurEffect();
		}

		public void BlockTouches()
		{
			for (int i = 0; i < _GraphicRaycasters.Count; i++)
			{
				_GraphicRaycasters[i].enabled = false;
			}
		}

		public void BlockNotDialogTouches()
		{
			BlockTouches();
			base.gameObject.GetComponent<GraphicRaycaster>().enabled = true;
		}

		public void UnBlockTouches()
		{
			for (int i = 0; i < _GraphicRaycasters.Count; i++)
			{
				_GraphicRaycasters[i].enabled = true;
			}
		}

		public void ShowEndFloorDialog(Action p_OnClose)
		{
			EndFloorDialog orCreateDialog = GetOrCreateDialog<EndFloorDialog>();
			orCreateDialog.Init(p_OnClose);
			orCreateDialog.Show();
			TurnOnBlurEffect();
		}

		public void ShowInfoDialog(List<DialogButtonData> p_buttons, string p_title, string p_text, bool p_moveToFront = false)
		{
			InfoDialogContent infoDialogContent = CreateBaseUIDialog<InfoDialogContent>();
			infoDialogContent.Init(p_buttons, p_title, p_text);
			infoDialogContent.Parent.Show(p_moveToFront);
			TurnOnBlurEffect();
			BlockNotDialogTouches();
		}

		public void ShowOptionsDialog()
		{
			OptionsDialog orCreateDialog = GetOrCreateDialog<OptionsDialog>();
			orCreateDialog.Init();
			orCreateDialog.Show();
			TurnOnBlurEffect();
			BlockNotDialogTouches();
		}

		public void ShowEnergyDialog(Action p_onCloseAfterRechargeAction)
		{
			EnergyDialogContent energyDialogContent = CreateBaseUIDialog<EnergyDialogContent>();
			energyDialogContent.Init(p_onCloseAfterRechargeAction);
			energyDialogContent.Parent.Show();
			TurnOnBlurEffect();
			BlockNotDialogTouches();
		}

		public void ShowSaleDialog(int p_saleID)
		{
			SaleDialogContent saleDialogContent = CreateBaseUIDialog<SaleDialogContent>();
			saleDialogContent.Init(p_saleID);
			saleDialogContent.Parent.Show();
			TurnOnBlurEffect();
			BlockNotDialogTouches();
		}

		public void ShowNewsDialog(string imagePath, Action<BaseDialog> imageAction, List<DialogButtonData> buttonsInfo)
		{
			NewsDialogContent newsDialogContent = CreateBaseUIDialog<NewsDialogContent>();
			newsDialogContent.Init(imagePath, imageAction, buttonsInfo);
			newsDialogContent.Parent.Show();
			TurnOnBlurEffect();
			BlockNotDialogTouches();
		}

		public void ShowChapterWindow(string title, string text, ChapterWindow.HideBy hideBy, ChapterWindow.ActionAfterHide actionAfterHide, ChapterWindowSettings settings, Action additionalAction = null)
		{
			ChapterWindow orCreateDialog = GetOrCreateDialog<ChapterWindow>();
			orCreateDialog.Init(title, text, hideBy, actionAfterHide, settings, additionalAction);
			orCreateDialog.Show();
			BlockNotDialogTouches();
		}

		public void ShowChapterWindow(string title, string text, string hideBy, string actionAfterHide, ChapterWindowSettings settings, Action additionalAction = null)
		{
			ChapterWindow orCreateDialog = GetOrCreateDialog<ChapterWindow>();
			orCreateDialog.Init(title, text, hideBy, actionAfterHide, settings, additionalAction);
			orCreateDialog.Show();
			BlockNotDialogTouches();
		}

		public void ShowPromoEffectDialog()
		{
			PromoEffectDialogContent promoEffectDialogContent = CreateBaseUIDialog<PromoEffectDialogContent>();
			promoEffectDialogContent.Init();
			promoEffectDialogContent.Parent.Show();
			TurnOnBlurEffect();
			BlockNotDialogTouches();
		}

		public void ShowPaymentDialog(string p_selectedGroup)
		{
			PaymentDialog orCreateDialog = GetOrCreateDialog<PaymentDialog>();
			orCreateDialog.Init(p_selectedGroup);
			orCreateDialog.Show();
			TurnOnBlurEffect();
			BlockNotDialogTouches();
		}

		public void ShowSelectCardDialog(GadgetItem p_item, CardsGroupAttribute p_card, Action<bool> p_answer)
		{
			SelectCardDialogContent selectCardDialogContent = CreateBaseUIDialog<SelectCardDialogContent>();
			selectCardDialogContent.Init(p_item, p_card, p_answer);
			selectCardDialogContent.Parent.Show();
			TurnOnBlurEffect();
			BlockNotDialogTouches();
		}

		public void ShowSimpleNotification(Notification.Parameters p_parameters)
		{
			SimpleNotification orCreateDialog = GetOrCreateDialog<SimpleNotification>();
			orCreateDialog.Init(p_parameters);
			orCreateDialog.Show();
		}

		public void ShowMissionNotification(Notification.Parameters p_parameters)
		{
			MissionNotification orCreateDialog = GetOrCreateDialog<MissionNotification>();
			orCreateDialog.Init(p_parameters);
			orCreateDialog.Show();
		}

		public void ShowTooltip(Tooltip.UISettings p_uiSettings)
		{
			Tooltip orCreateDialog = GetOrCreateDialog<Tooltip>();
			orCreateDialog.Init(p_uiSettings);
			orCreateDialog.Show();
		}

		public void ShowQuestStartDialog(string p_title, string p_objectiveText, Quest p_quest, Action p_onClose, bool p_noDetails)
		{
			QuestStartDialogContent questStartDialogContent = CreateBaseUIDialog<QuestStartDialogContent>();
			questStartDialogContent.Init(p_title, p_objectiveText, p_quest, p_onClose, p_noDetails);
			questStartDialogContent.Parent.Show();
			TurnOnBlurEffect();
			BlockNotDialogTouches();
		}

		public void ShowQuestCompleteDialog(string p_title, Quest p_quest, Action p_onClose)
		{
			QuestCompleteDialogContent questCompleteDialogContent = CreateBaseUIDialog<QuestCompleteDialogContent>();
			questCompleteDialogContent.Init(p_title, p_quest, p_onClose);
			questCompleteDialogContent.Parent.Show();
			TurnOnBlurEffect();
			BlockNotDialogTouches();
		}

		public void ShowQuestTalkingDialog(string p_title, string p_text, string p_buttonText, string p_image, Action p_onClose)
		{
			QuestTalkingDialogContent questTalkingDialogContent = CreateBaseUIDialog<QuestTalkingDialogContent>();
			questTalkingDialogContent.Init(p_title, p_text, p_buttonText, p_image, p_onClose);
			questTalkingDialogContent.Parent.Show();
			TurnOnBlurEffect();
			BlockNotDialogTouches();
		}

		public void ShowQuestTalkingDialog(string p_title, string p_text, string p_image, List<DialogButtonData> p_buttons)
		{
			QuestTalkingDialogContent questTalkingDialogContent = CreateBaseUIDialog<QuestTalkingDialogContent>();
			questTalkingDialogContent.Init(p_title, p_text, p_image, p_buttons);
			questTalkingDialogContent.Parent.Show();
			TurnOnBlurEffect();
			BlockNotDialogTouches();
		}

		public void ShowBundleDownloadDialog()
		{
			BundleDownloadDialogContent bundleDownloadDialogContent = CreateBaseUIDialog<BundleDownloadDialogContent>();
			bundleDownloadDialogContent.Init();
			bundleDownloadDialogContent.Parent.Show();
			TurnOnBlurEffect();
			BlockNotDialogTouches();
		}

		public void ShowMissionsDialog(bool p_isWithContinueButton)
		{
			MissionsDialog orCreateDialog = GetOrCreateDialog<MissionsDialog>();
			orCreateDialog.Init(p_isWithContinueButton);
			orCreateDialog.Show();
			TurnOnBlurEffect();
		}

		public void CloseDialog<T>() where T : BaseDialog
		{
			string key = typeof(T).Name;
			if (_CreatedDialogs.ContainsKey(key))
			{
				_CreatedDialogs[key].Dismiss();
			}
		}

		public T GetOrCreateDialog<T>() where T : BaseDialog
		{
			BaseDialog orCreateDialog = GetOrCreateDialog(typeof(T).Name);
			return (!(orCreateDialog != null)) ? ((T)null) : (orCreateDialog as T);
		}

		public BaseDialog GetOrCreateDialog(string p_typeName)
		{
			BaseDialog baseDialog;
			if (_CreatedDialogs.ContainsKey(p_typeName))
			{
				baseDialog = _CreatedDialogs[p_typeName];
				if (baseDialog != null)
				{
					return baseDialog;
				}
				_CreatedDialogs.Remove(p_typeName);
			}
			baseDialog = UnityEngine.Object.Instantiate(GetPrefabByType(p_typeName)).GetComponent<BaseDialog>();
			baseDialog.gameObject.SetActive(false);
			if (p_typeName != typeof(Tooltip).Name)
			{
				baseDialog.transform.SetParent(base.transform, false);
			}
			_CreatedDialogs.Add(p_typeName, baseDialog);
			return baseDialog;
		}

		private GameObject GetPrefabByType(string p_type)
		{
			if (_PrefabsByType.ContainsKey(p_type))
			{
				return _PrefabsByType[p_type];
			}
			return null;
		}

		private T CreateBaseUIDialog<T>() where T : DialogContent
		{
			string text = typeof(T).Name;
			if (_CreatedDialogs.ContainsKey(text))
			{
				return (_CreatedDialogs[text] as BaseUIDialog).Content as T;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(_BaseUIDialogPrefab);
			gameObject.transform.SetParent(base.transform, false);
			BaseUIDialog component = gameObject.GetComponent<BaseUIDialog>();
			T result = component.Init<T>(GetPrefabByType(text));
			_CreatedDialogs.Add(text, component);
			return result;
		}
	}
}
