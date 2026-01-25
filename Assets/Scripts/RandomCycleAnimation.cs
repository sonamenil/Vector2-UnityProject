using Nekki;
using UnityEngine;

public class RandomCycleAnimation : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		float value = NekkiMath.randomFloat(0f, stateInfo.length);
		float value2 = NekkiMath.randomFloat(1f, 1.4f);
		animator.SetFloat("startTime", value);
		animator.SetFloat("speed", value2);
	}
}
