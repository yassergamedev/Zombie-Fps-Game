using UnityEngine;

public class DisableRootMotionOnState : StateMachineBehaviour
{
    // This will be called when the state machine starts evaluating this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Disable root motion
        animator.applyRootMotion = false;
    }

    // This will be called when the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Re-enable root motion after exiting the state
        animator.applyRootMotion = true;
    }
}
