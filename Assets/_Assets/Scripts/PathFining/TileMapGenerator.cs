using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapGenerator : MonoBehaviour
{
    [SerializeField, Range(1.05f, 3)]
    float _neigborsTheshold = 1.5f;
    [SerializeField]
    bool _drawNeighborRange = false;
    
    public List<Tile> tiles;

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
