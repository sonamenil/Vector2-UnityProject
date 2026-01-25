using UnityEngine;
using UnityEngine.EventSystems;

namespace BlendModes
{
	public class DemoBall : MonoBehaviour, IDragHandler, IEventSystemHandler
	{
		public void OnDrag(PointerEventData eventData)
		{
			base.transform.position = eventData.position;
		}
	}
}
