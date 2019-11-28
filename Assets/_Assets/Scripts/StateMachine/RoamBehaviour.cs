using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoamBehaviour : StateMachineBehaviour
{
    StateMachine _stateMachine;
    Tank _tank;
    TileMapGenerator _generator;

    public bool chasing = false;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_tank)
            _tank = animator.GetComponent<Tank>();
        if (!_stateMachine)
            _stateMachine = animator.GetComponent<StateMachine>();
        if (!_generator)
            _generator = FindObjectOfType<TileMapGenerator>();  // Finds first intance in scene.
    }

    void MoveToNewRandomTile(Transform t)
    {
        // Remove the oldest tile in recent list as we're adding a new one.
        if (_stateMachine.recentTiles.Count >= _stateMachine.recentListLength) 
            _stateMachine.recentTiles.RemoveAt(0);

        // Get a list of all neighbors, pick a random one we haven't been on.
        List<Tile> neighbors = new List<Tile> (_generator.FindClosestTile(t).neighbors); 
        neighbors = neighbors.OrderBy(x => Random.value).ToList(); // Shuffle our neigbor list copy.
        List<Tile> options = new List<Tile>();
        foreach (Tile tile in neighbors)
        {
            if (!_stateMachine.recentTiles.Contains(tile))
            {
                _tank.waypointList.Add(tile.transform);
                _stateMachine.recentTiles.Add(tile);
                return;
            }
            else
            {
                options.Add(tile);
            }
        }
        // If we've gotten to this point, we are surrounded by tiles we've been on, pick the oldest one.
        foreach (Tile tile in _stateMachine.recentTiles)
        {
            if (options.Contains(tile))
            {
                _tank.waypointList.Add(tile.transform);
                _stateMachine.recentTiles.Add(tile);
                return;
            }
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_tank.waypointList.Count == 0 && !chasing)
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
