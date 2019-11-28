using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(TileMapGenerator))]
public class PathFinder : MonoBehaviour
{
    TileMapGenerator _generator;

    private void Awake() 
    {
        _generator = GameObject.Find("TileMap").GetComponent<TileMapGenerator>();
    }


    public List<Transform> GetPath(Transform startEntity, Transform endEntity)
    { 
        Tile startTile = _generator.FindClosestTile(startEntity);
        Tile endTile = _generator.FindClosestTile(endEntity);
        List <Transform> path = GetPath(startTile, endTile);
        //path.Add(endEntity);
        return path;
    }

    public List<Transform> GetPath(Tile startNode, Tile endNode) 
    {
        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();
        Tile current = null;

        _generator.ResetTiles();
        startNode.g = 0;
        openList.Add(startNode);
        

        int i = 0;
        while (openList.Count > 0) 
        {
            openList.Sort();
            current = openList[openList.Count-1];

            if (current == endNode) // If destination on the left, stop searching,
                break;

            openList.Remove(current);

            int addedNeigbors = 0;
            foreach (Tile t in current.neighbors) {

                t.h = Vector3.Magnitude(t.transform.position - endNode.transform.position);

                float tentativeG = current.g + Vector3.Magnitude(current.transform.position - t.transform.position);
                if (tentativeG <= t.g) 
                {
                    t.par = current;
                    closedList.Add(current);
                    t.g = tentativeG;
                    t.f = t.g + t.h;
                    if (!openList.Contains(t)) 
                    {
                        openList.Add(t);
                    }
                }
            }

            Debug.Log("New iteration: " + i 
                + " open: " + openList.Count 
                + " closed: " + closedList.Count 
                + " Added neigbors: " + addedNeigbors
                + "Smallest f:  " /*+ openList[0].f*/);
            i++;
        } 
        List<Transform> path = new List<Transform>();
        //foreach (Tile t in closedList)
        //   path.Add(t.transform);
        bool children = true;
        Tile tile1 = endNode;
        while (children)
        {
            path.Add(tile1.transform);
            tile1 = tile1.par;
            if (!tile1) children = false;
        }
        path.Reverse();

        if (current == endNode) // If destination on the left, stop searching,
            Debug.Log("Done. Path length: " + path.Count);
        else
            Debug.Log("Done. No path possible! Closest path length: " + path.Count);

        return path;
    }
}
