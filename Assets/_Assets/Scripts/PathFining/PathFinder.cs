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

        while (openList.Count > 0) 
        {
            foreach (Tile t in current.neighbors)
            {
                t.h = Vector3.SqrMagnitude(t.transform.position - endNode.transform.position);
                t.f = t.h + t.Weight;
                openList.Add(t);
            }

            openList.Sort();

            closedList.Add(current);
            current = openList[0];
            openList.RemoveAt(0);

        }

        List<Transform> path = new List<Transform>();
        foreach (Tile t in closedList)
            path.Add(t.transform);

        return path;
    }
}
