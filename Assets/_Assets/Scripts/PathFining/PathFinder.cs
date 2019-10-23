using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TileMapGenerator))]
public class PathFinder : MonoBehaviour
{
    TileMapGenerator _generator;

    private void Awake() 
    {
        _generator = GetComponent<TileMapGenerator>();
    }

    public Transform[] GetPath(Transform startNode, Transform endNode) 
    {
        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();
        Tile current = startNode.GetComponent<Tile>();
        List<float> h = new List<float>();

        for (int i = 0; i < _generator.tiles.Count; i++)
        {
            h.Add(Vector3.SqrMagnitude(openList[i].transform.position - endNode.position));
        }

        while (openList.Count > 0) 
        {

        }

        Transform[] path = new Transform[0];
        return path;
    }
}
