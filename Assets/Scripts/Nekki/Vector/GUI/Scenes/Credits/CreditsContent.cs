using System.Collections.Generic;
using Nekki.Yaml;
using UnityEngine;

namespace Nekki.Vector.GUI.Scenes.Credits
{
	public abstract class CreditsContent
	{
		public static List<CreditsContent> CreateList(Sequence p_node)
		{
			if (p_node == null)
			{
				return null;
			}
			List<CreditsContent> list = new List<CreditsContent>();
			foreach (Mapping item in p_node)
			{
				CreditsContent creditsContent = Create(item);
				if (creditsContent != null)
				{
					list.Add(creditsContent);
				}
			}
			return (list.Count == 0) ? null : list;
		}

		public static CreditsContent Create(Mapping p_node)
		{
			if (p_node.GetText("Title") != null)
			{
				return new CC_Title(p_node);
			}
			if (p_node.GetSequence("Data") != null)
			{
				return new CC_Data(p_node);
			}
			if (p_node.GetText("Space") != null)
			{
				return new CC_Space(p_node);
			}
			DebugUtils.Dialog("Unknown CreditsContent node: " + p_node.ToString(), true);
			return null;
		}

		public abstract void GenerateContentUI(Transform p_parent);

		protected GameObject CreateGO<T>(Transform p_parent)
		{
			GameObject gameObject = Object.Instantiate(CreditsController.Current.GetPrefabByType(typeof(T)));
			gameObject.transform.SetParent(p_parent, false);
			return gameObject;
		}
	}
}
