using System.Collections.Generic;
using DG.Tweening;
using Nekki.Yaml;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.Vector.GUI.Scenes.Credits
{
	public abstract class CreditsBlock
	{
		protected List<CreditsContent> _Content = new List<CreditsContent>();

		protected CreditsBlock(Mapping p_node)
		{
			_Content = CreditsContent.CreateList(p_node.GetSequence("Content"));
		}

		public static List<CreditsBlock> CreateList(Nekki.Yaml.Sequence p_node)
		{
			if (p_node == null)
			{
				return null;
			}
			List<CreditsBlock> list = new List<CreditsBlock>();
			foreach (Mapping item in p_node)
			{
				CreditsBlock creditsBlock = Create(item);
				if (creditsBlock != null)
				{
					list.Add(creditsBlock);
				}
			}
			return (list.Count == 0) ? null : list;
		}

		public static CreditsBlock Create(Mapping p_node)
		{
			string stringValue = YamlUtils.GetStringValue(p_node.GetText("BlockType"), string.Empty);
			if (stringValue == "Fade")
			{
				return new CB_Fade(p_node);
			}
			if (stringValue == "Scroll")
			{
				return new CB_Scroll(p_node);
			}
			DebugUtils.Dialog("Unknown CreditsBlock node: " + p_node.ToString(), true);
			return null;
		}

		public abstract void GenerateContentUI(Transform p_parent, DG.Tweening.Sequence p_sequnce);

		protected GameObject CreateGO<T>(Transform p_parent)
		{
			GameObject gameObject = Object.Instantiate(CreditsController.Current.GetPrefabByType(typeof(T)));
			gameObject.transform.SetParent(p_parent, false);
			foreach (CreditsContent item in _Content)
			{
				item.GenerateContentUI(gameObject.transform);
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
			gameObject.SetActive(false);
			return gameObject;
		}
	}
}
