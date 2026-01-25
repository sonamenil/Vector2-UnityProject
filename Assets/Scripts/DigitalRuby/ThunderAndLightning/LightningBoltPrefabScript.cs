using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public class LightningBoltPrefabScript : LightningBoltPrefabScriptBase
	{
		[Tooltip("The source game object")]
		public GameObject Source;

		[Tooltip("The destination game object")]
		public GameObject Destination;

		public override void CreateLightningBolt(LightningBoltParameters parameters)
		{
			if (!(Source == null) && !(Destination == null))
			{
				parameters.Start = Source.transform.position;
				parameters.End = Destination.transform.position;
				base.CreateLightningBolt(parameters);
			}
		}
	}
}
