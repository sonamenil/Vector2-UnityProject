using System.Collections.Generic;
using Nekki.Vector.Core.GameManagement;
using UnityEngine;

namespace Nekki.Vector.GUI.MainScene
{
	public class FillersPanel : MonoBehaviour
	{
		[SerializeField]
		private GameObject _FillerUIPrefab;

		private List<FillerUI> _Fillers = new List<FillerUI>();

		public void Init(StarterPackItem p_item)
		{
			ClearChildFillers();
			List<BuffGroupAttribute> buffsByStarterPack = BuffGroupAttribute.GetBuffsByStarterPack(p_item.Name);
			if (buffsByStarterPack != null)
			{
				for (int i = 0; i < buffsByStarterPack.Count; i++)
				{
					FillerUI component = Object.Instantiate(_FillerUIPrefab).GetComponent<FillerUI>();
					component.transform.SetParent(base.transform, false);
					component.Init(buffsByStarterPack[i], OnFillerTap);
					_Fillers.Add(component);
				}
			}
		}

		private void ClearChildFillers()
		{
			for (int i = 0; i < _Fillers.Count; i++)
			{
				Object.Destroy(_Fillers[i].gameObject);
			}
			_Fillers.Clear();
		}

		public void OnFillerTap(FillerUI p_Filler)
		{
		}
	}
}
