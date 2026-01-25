using Nekki;
using UnityEngine;

public class IdleRandomizer : StateMachineBehaviour
{
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.SetInteger("idleselector", NekkiMath.randomInt(0, 7));
	}
}
