using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamBehaviour : StateMachineBehaviour
{
    StateMachine stateMachine;
    StateMachineControl control;


    void MoveToNewRandomTile()
    {
        // if length bigger than recentLength
    }

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!stateMachine)
            stateMachine = animator.GetComponent<StateMachine>();
        if (!control)
            control = animator.GetComponent<StateMachineControl>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!control.moveToTrasform)
        {
            MoveToNewRandomTile();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
