using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoamBehaviour : StateMachineBehaviour
{
    StateMachine _stateMachine;
    StateMachineControl _control;
    TileMapGenerator _generator;

    void MoveToNewRandomTile(Transform t)
    {
        //if length bigger than recentLength
        if (_stateMachine.recentTiles.Count >= _stateMachine.recentListLength)
            _stateMachine.recentTiles.RemoveAt(0);
        List<Tile> neighbors = new List<Tile> (_generator.FindClosestTile(t).neighbors);
        neighbors = neighbors.OrderBy(x => Random.value).ToList(); // Shuffle our neigbor list copy.
        List<Tile> options = new List<Tile>();
        foreach (Tile tile in neighbors)
        {
            if (!_stateMachine.recentTiles.Contains(tile))
            {
                _control.goToList.Add(tile.transform);
                _stateMachine.recentTiles.Add(tile);
                return;
            }
            else
            {
                options.Add(tile);
            }
        }
        foreach (Tile tile in _stateMachine.recentTiles)
        {
            if (options.Contains(tile))
            {
                _control.goToList.Add(tile.transform);
                _stateMachine.recentTiles.Add(tile);
                return;
            }
        }
    }

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_stateMachine)
            _stateMachine = animator.GetComponent<StateMachine>();
        if (!_control)
            _control = animator.GetComponent<StateMachineControl>();
        if (!_generator)
            _generator = FindObjectOfType<TileMapGenerator>();  // Finds first intance in scene.
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_control.goToList.Count == 0)
        {
            MoveToNewRandomTile(animator.transform);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
