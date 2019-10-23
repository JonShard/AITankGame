using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    [SerializeField, Range(0.01f, 0.5f)]
    private float _gizmoSize = 0.2f;
    [SerializeField, Range(0.01f, 1)]
    private float _gizmoHeight = 0.2f;

    public enum TileType {Traversable, NonTraversable}
    public TileType type;
    public float weight = 1;
    public List<Transform> neighbors;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (neighbors != null)
        {
            foreach (Transform t in neighbors)
            {
                Gizmos.DrawLine(transform.position + new Vector3(0, _gizmoHeight, 0), t.position + new Vector3(0, _gizmoHeight, 0));
            }
        }
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + new Vector3(0, _gizmoHeight, 0), _gizmoSize);
    }
#endif
}
