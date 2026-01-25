using System.Collections.Generic;
using DG.Tweening;
using Nekki.Vector.Core;
using Nekki.Vector.Core.Localization;
using Nekki.Vector.Core.User;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Run
{
	public class FloatingText : MonoBehaviour
	{
		private static GameObject _Prefab;

		private static RectTransform _WorldParent;

		private static RectTransform _ScreenParent;

		private static Dictionary<string, FloatingText> _FloatingTextsCache = new Dictionary<string, FloatingText>();

		private static List<FloatingText> _ActiveFloatingTexts = new List<FloatingText>();

		public static string AtlasesPath = "UI/Atlases/";

		[SerializeField]
		private int DefaultFontSize = 80;

		[SerializeField]
		private Color DefaultFontColor = Color.green;

		[SerializeField]
		private FontStyle DefaultIsItalic;

		[SerializeField]
		private Color DefaultBorderColor = Color.blue;

		[SerializeField]
		private float DefaultTime = 0.8f;

		[SerializeField]
		private float DefaultDelay = 0.4f;

		[SerializeField]
		private FT_ImageAlign DefaultImageAlign;

		public int FontSize;

		public Color FontColor;

		public FontStyle isItalic;

		public Color BorderColor;

		public float Time;

		public float Delay;

		public Vector2 Shift;

		public string Text;

		public FT_OriginType OriginType;

		public Vector3 Position;

		public FT_ImageAlign ImageAlign;

		public Color ImageColor = Color.white;

		public int ImageWidth = -1;

		public int ImageHeight = -1;

		[SerializeField]
		private LabelAlias _Label;

		[SerializeField]
		private LayoutElement _LabelLayout;

		[SerializeField]
		private ResolutionImage _Image;

		[SerializeField]
		private Outline _LabelOutline;

		[SerializeField]
		private LayoutElement _ImageLayoutElement;

		[SerializeField]
		private CanvasGroup _CanvasGroup;

		private Sequence _MoveSequence;

		private Sequence _AlphaSequence;

		public string Image
		{
			get
			{
				return _Image.SpriteName;
			}
			set
			{
				_Image.SpriteName = value.Replace("Run:", string.Empty);
			}
		}

		public static FloatingText Create(string p_type, string p_triggerName, string p_keyForCaching = null)
		{
			switch (p_type)
			{
			case "Local":
				return Create(FT_OriginType.Local, p_triggerName, p_keyForCaching);
			case "Screen":
				return Create(FT_OriginType.Screen, p_triggerName, p_keyForCaching);
			case "Gadget":
				return Create(FT_OriginType.Gadget, p_triggerName, p_keyForCaching);
			case "Subtitles":
				return Create(FT_OriginType.Subtitles, p_triggerName, p_keyForCaching);
			default:
				return null;
			}
		}

		public static FloatingText Create(FT_OriginType p_type, string p_triggerName, string p_keyForCaching = null)
		{
			FloatingText floatingText;
			if (p_keyForCaching != null)
			{
				floatingText = TryGetFromCache(p_keyForCaching);
				if (floatingText != null)
				{
					return floatingText;
				}
			}
			floatingText = InstatiateGo(p_type, p_triggerName);
			if (p_keyForCaching != null)
			{
				_FloatingTextsCache.Add(p_keyForCaching, floatingText);
				FloatingText floatingText2 = floatingText;
				floatingText2.name = floatingText2.name + "[c: " + p_keyForCaching + "]";
			}
			return floatingText;
		}

		private static FloatingText TryGetFromCache(string p_keyForCaching)
		{
			if (_FloatingTextsCache.ContainsKey(p_keyForCaching))
			{
				return _FloatingTextsCache[p_keyForCaching];
			}
			return null;
		}

		private static FloatingText InstatiateGo(FT_OriginType p_type, string p_triggerName)
		{
			InitStaticData();
			GameObject gameObject = Object.Instantiate(_Prefab);
			gameObject.name = string.Format("FloatingText[Trigger: {0}]", p_triggerName);
			if (p_type == FT_OriginType.Local)
			{
				gameObject.transform.SetParent(_WorldParent, false);
			}
			else
			{
				gameObject.transform.SetParent(_ScreenParent, false);
			}
			FloatingText component = gameObject.GetComponent<FloatingText>();
			component.OriginType = p_type;
			return component;
		}

		private static void InitStaticData()
		{
			if (_Prefab == null)
			{
				_Prefab = Resources.Load<GameObject>("UI/Prefabs/Run/FloatingText");
			}
			if (_WorldParent == null)
			{
				_WorldParent = GameObject.Find("[FloatingTexts]/World").GetComponent<RectTransform>();
			}
			if (_ScreenParent == null)
			{
				_ScreenParent = GameObject.Find("[FloatingTexts]/Screen").GetComponent<RectTransform>();
			}
		}

		public static void ResetStaticData()
		{
			_WorldParent = null;
			_ScreenParent = null;
			_FloatingTextsCache.Clear();
		}

		public static Vector3 ConvertWorldToScreenCoords(Vector3 p_worldPos)
		{
			Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, p_worldPos);
			Vector2 localPoint = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(_ScreenParent, screenPoint, null, out localPoint);
			return new Vector3(localPoint.x / _ScreenParent.sizeDelta.x, localPoint.y / _ScreenParent.sizeDelta.y, 0f);
		}

		private void Awake()
		{
			Reset();
			RunMainController.OnPause += OnGamePaused;
		}

		private void OnDestroy()
		{
			RunMainController.OnPause -= OnGamePaused;
			_ActiveFloatingTexts.Remove(this);
		}

		public void Play()
		{
			if (OriginType != FT_OriginType.Subtitles || DataLocal.Current.Settings.SubtitlesOn)
			{
				_ActiveFloatingTexts.Add(this);
				SetupUI();
				base.gameObject.SetActive(true);
				SetupTweens();
			}
		}

		private void SetupUI()
		{
			if (OriginType == FT_OriginType.Local)
			{
				base.transform.localScale = new Vector3(1f, -1f, 1f);
				base.transform.position = Position;
			}
			else
			{
				base.transform.localScale = new Vector3(1f, 1f, 1f);
				Shift = new Vector2(Shift.x * _ScreenParent.sizeDelta.x, Shift.y * _ScreenParent.sizeDelta.y);
				base.transform.localPosition = new Vector3(Position.x * _ScreenParent.sizeDelta.x, Position.y * _ScreenParent.sizeDelta.y, 0f);
				_Label.resizeTextForBestFit = true;
				_Label.horizontalOverflow = HorizontalWrapMode.Wrap;
				_Label.verticalOverflow = VerticalWrapMode.Truncate;
				_LabelLayout.preferredHeight = 350f;
				_LabelLayout.preferredWidth = 2048f;
			}
			_Label.fontSize = FontSize;
			_Label.color = FontColor;
			_Label.fontStyle = isItalic;
			_LabelOutline.effectColor = BorderColor;
			_Label.SetAlias(Text);
			bool flag = _Image.sprite != null;
			if (flag)
			{
				SetImageSize();
				SetImageAlign();
				_Image.color = ImageColor;
			}
			_Image.gameObject.SetActive(flag);
		}

		private void SetImageSize()
		{
			_ImageLayoutElement.preferredWidth = ((ImageWidth == -1) ? _Image.sprite.rect.width : ((float)ImageWidth));
			_ImageLayoutElement.preferredHeight = ((ImageHeight == -1) ? _Image.sprite.rect.height : ((float)ImageHeight));
		}

		private void SetImageAlign()
		{
			if (ImageAlign == FT_ImageAlign.Right)
			{
				_Label.rectTransform.SetSiblingIndex(0);
				_Image.rectTransform.SetSiblingIndex(1);
			}
			else
			{
				_Image.rectTransform.SetSiblingIndex(0);
				_Label.rectTransform.SetSiblingIndex(1);
			}
		}

		private void SetupTweens()
		{
			_CanvasGroup.alpha = 1f;
			_MoveSequence = DOTween.Sequence();
			_MoveSequence.Append(base.transform.DOLocalMove(base.transform.localPosition + new Vector3(Shift.x, Shift.y, 0f), Time + Delay));
			_AlphaSequence = DOTween.Sequence();
			_AlphaSequence.AppendInterval(Delay);
			_AlphaSequence.Append(_CanvasGroup.DOFade(0f, Time));
			_AlphaSequence.OnComplete(delegate
			{
				_ActiveFloatingTexts.Remove(this);
				Reset();
			});
			_MoveSequence.Play();
			_AlphaSequence.Play();
		}

		private void Reset()
		{
			FontSize = DefaultFontSize;
			FontColor = DefaultFontColor;
			isItalic = DefaultIsItalic;
			BorderColor = DefaultBorderColor;
			Time = DefaultTime;
			Delay = DefaultDelay;
			Image = string.Empty;
			ImageAlign = DefaultImageAlign;
			Text = string.Empty;
			ImageColor = Color.white;
			ImageWidth = (ImageHeight = -1);
			_MoveSequence = null;
			_AlphaSequence = null;
			_Label.resizeTextForBestFit = false;
			_Label.horizontalOverflow = HorizontalWrapMode.Overflow;
			_Label.verticalOverflow = VerticalWrapMode.Overflow;
			_LabelLayout.preferredHeight = -1f;
			_LabelLayout.preferredWidth = -1f;
			base.gameObject.SetActive(false);
		}

		public void Stop()
		{
			_MoveSequence.Kill();
			_AlphaSequence.Kill();
			Reset();
		}

		private void OnGamePaused(bool p_value)
		{
			if (p_value)
			{
				_MoveSequence.Pause();
				_AlphaSequence.Pause();
			}
			else
			{
				_MoveSequence.Play();
				_AlphaSequence.Play();
			}
		}

		public static void StopAllActive()
		{
			foreach (FloatingText activeFloatingText in _ActiveFloatingTexts)
			{
				activeFloatingText.Stop();
			}
		}

		public static void FreeAllActive()
		{
			_ActiveFloatingTexts.Clear();
		}
	}
}
