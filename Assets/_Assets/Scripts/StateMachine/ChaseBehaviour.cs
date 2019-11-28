using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseBehaviour : StateMachineBehaviour
{
   
    Transform lastKnownPos;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       

        if (animator.GetComponent<StateMachine>().target)
        {
            lastKnownPos = animator.GetComponent<StateMachine>().target.transform;

            animator.GetComponent<Tank>().FindPath(animator.transform, lastKnownPos);
        }
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        // _control.goToList.Add(tile.transform);

        if (animator.GetComponent<StateMachine>().target)
        {
            //lastKnownPos = animator.GetComponent<StateMachine>().target.transform;
            
        }


        }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
