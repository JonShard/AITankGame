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
        path.Add(endEntity);
        return path;
    }

    public List<Transform> GetPath(Tile startNode, Tile endNode) 
    {
        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();
        Tile current = startNode;
        closedList.Add(current);

        int i = 0;
        do {
            if (current == endNode) // If destination on the left, stop searching,
                break;

            i++;
            int addedNeigbors = 0;
            foreach (Tile t in current.neighbors) {
                if (closedList.Contains(t))
                    continue;
                t.h = Vector3.SqrMagnitude(t.transform.position - endNode.transform.position);
                t.f = t.h + t.Weight;
                openList.Add(t);
                addedNeigbors++;
            }

            openList.Sort();
            openList.Reverse();

            Debug.Log("New iteration: " + i 
                + " open: " + openList.Count 
                + " closed: " + closedList.Count 
                + " Added neigbors: " + addedNeigbors
                + "Smallest f: " + openList[0].f);

            closedList.Add(current);
            current = openList[0];
            openList.RemoveAt(0);

        } while (openList.Count > 0);
        List<Transform> path = new List<Transform>();
        foreach (Tile t in closedList)
            path.Add(t.transform);

        Debug.Log("Done. Path length: " + path.Count);
        return path;
    }
}
