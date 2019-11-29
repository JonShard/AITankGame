using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBehaviour : StateMachineBehaviour
{
    StateMachine _stateMachine;
    Tank _tank;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_stateMachine)
            _stateMachine = animator.GetComponent<StateMachine>();
        if (!_tank)
            _tank = animator.GetComponent<Tank>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_stateMachine.target) _tank.aimAtTrasform = _stateMachine.target.transform;
        _tank.FireGun();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _tank.aimAtTrasform = null;
    }
}
