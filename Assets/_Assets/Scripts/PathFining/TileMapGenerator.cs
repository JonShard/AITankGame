using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapGenerator : MonoBehaviour
{
    [SerializeField, Range(1.05f, 3)]
    float _neigborsTheshold = 1.5f;
    [SerializeField]
    bool _drawNeighborRange = false;
    
    [HideInInspector] public List<Tile> tiles;


    private void Start() {
        SetNeigbors();
    }

    public Tile FindClosestTile(Transform point)
    {
        float shortestSquareDist = Mathf.Infinity;
        Tile bestCandidate = null;
        if (tiles.Count == 0)
        {
            Debug.LogWarning("Tried to get closest tile to a point, but tiles is empty.");
            return bestCandidate;
        }

        foreach (Tile t in tiles)
        {
            float dist = (t.transform.position - point.position).sqrMagnitude;
            if (dist < shortestSquareDist)
            {
                shortestSquareDist = dist;
                bestCandidate = t;
            }
        }
        return bestCandidate;
    }

    void ClearNeigbors() 
    {
        tiles = new List<Tile>(GetComponentsInChildren<Tile>());

        foreach (Tile t in tiles) {
            t.neighbors.Clear();
        }
    }

    [ContextMenu("Configure tiles")]
    void SetNeigbors() {
        ClearNeigbors();
        foreach (Tile t in tiles) {
            foreach (Tile tt in tiles) {
                if (t == tt)
                    continue;
                if (Vector3.SqrMagnitude(t.transform.position - tt.transform.position)
                    < _neigborsTheshold * _neigborsTheshold) {
                    t.neighbors.Add(tt);
                }
            }
        }
    }

    [ContextMenu("GenerateRandomMap")]
    void GenerateMap()
    {

    }

    [ContextMenu("Generate walls")]
    void GenerateWalls() 
    {

    }

    private void OnDrawGizmos()
    {
        if (_drawNeighborRange)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position + new Vector3(0, 0.05f, 0), _neigborsTheshold);
        }
    }
}
