using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(TileMapGenerator))]
public class PathFinder : MonoBehaviour
{
    private TileMapGenerator _generator;
    private List<Tile> _openList = new List<Tile>();
    private List<Tile> _closedList = new List<Tile>();
    private List<Transform> _path = new List<Transform>();

    private void Awake() 
    {
        _generator = GameObject.Find("TileMap").GetComponent<TileMapGenerator>();
    }


    public List<Transform> GetPath(Vector3 startPos, Vector3 endPos)
    { 
        Tile startTile = _generator.FindClosestTile(startPos);
        Tile endTile = _generator.FindClosestTile(endPos);
        List <Transform> path = GetPath(startTile, endTile);
        //path.Add(endEntity);
        return path;
    }



    public List<Transform> GetPath(Tile startNode, Tile endNode) 
    {
        Tile current = null;
        _openList.Clear();
        _closedList.Clear();
        _path.Clear();

        _generator.ResetTiles();
        startNode.g = 0;
        _openList.Add(startNode);
        

        int i = 0;
        while (_openList.Count > 0) 
        {
            _openList.Sort();
            current = _openList[_openList.Count-1];

            if (current == endNode) // If destination on the left, stop searching,
                break;

            _openList.Remove(current);

            int addedNeigbors = 0;
            foreach (Tile t in current.neighbors) {

                t.h = Vector3.Magnitude(t.transform.position - endNode.transform.position);

                float tentativeG = current.g + Vector3.Magnitude(current.transform.position - t.transform.position);
                if (tentativeG <= t.g) 
                {
                    t.parent = current;
                    _closedList.Add(current);
                    t.g = tentativeG;
                    t.f = t.g + t.h;
                    if (!_openList.Contains(t)) 
                    {
                        _openList.Add(t);
                    }
                }
            }

            Debug.Log("New iteration: " + i 
                + " open: " + _openList.Count 
                + " closed: " + _closedList.Count 
                + " Added neigbors: " + addedNeigbors
                + "Smallest f:  " /*+ openList[0].f*/);
            i++;
        }
        //foreach (Tile t in closedList)
        //   path.Add(t.transform);
        bool children = true;
        Tile tile1 = endNode;
        while (children)
        {
            _path.Add(tile1.transform);
            tile1 = tile1.parent;
            if (!tile1) children = false;
        }
        _path.Reverse();

        if (current == endNode) // If destination on the left, stop searching,
            Debug.Log("Done. Path length: " + _path.Count);
        else
            Debug.Log("Done. No path possible! Closest path length: " + _path.Count);

        return _path;
    }
}
