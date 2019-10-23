using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapGenerator : MonoBehaviour
{
    [SerializeField, Range(1.05f, 3)]
    float _neigborsTheshold = 1.5f;
    [SerializeField]
    bool _drawNeighborRange = false;


    [ContextMenu("GenerateRandomMap")]
    void GenerateMap()
    {

    }

    [ContextMenu("Configure tiles")]
    void SetNeigbors()
    {
        Tile[] tiles = GetComponentsInChildren<Tile>(); 
        foreach (Tile t in tiles)
        {
            // Find all tiles within range. 
            // Add them to the list.
            //t.neighbors.Add()
        }
    }

    [ContextMenu("Generate walls")]
    void GenerateWalls()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
