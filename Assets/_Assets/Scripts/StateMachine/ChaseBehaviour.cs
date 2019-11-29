using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseBehaviour : StateMachineBehaviour
{
    StateMachine _stateMachine;
    Tank _tank;
    TileMapGenerator _generator;
    PathFinder _pathFinder;
    float _pathfindingCooldown = 1; // update path once per second.

    float _cooldown = 0;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_tank)
            _tank = animator.GetComponent<Tank>();
        if (!_stateMachine)
            _stateMachine = animator.GetComponent<StateMachine>();
        if (!_generator)
            _generator = FindObjectOfType<TileMapGenerator>();  // Finds first intance in scene.    
        if (!_pathFinder)
            _pathFinder = _generator.GetComponent<PathFinder>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _cooldown -= Time.deltaTime;
        if (_cooldown < 0)
        {
            _tank.waypointList = _pathFinder.GetPath(_tank.transform.position, _stateMachine.lastKnowPos);
            _cooldown = _pathfindingCooldown;
        }
        if (Vector3.Magnitude(_tank.transform.position - _stateMachine.lastKnowPos) < 1)
        {
            _stateMachine.hasTarget = false;    // If we reach the last know position but we havent re-aquired target in state machine, go back to roaming.
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
