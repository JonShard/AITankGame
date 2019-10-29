using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, IComparable<Tile> {

    public enum TileType { Traversable, NonTraversable }

    [SerializeField] private float weight = 1;
    public float Weight { get { return weight; } }

    [SerializeField, Range(0.01f, 0.5f)] private float _gizmoSize = 0.2f;
    [SerializeField, Range(0.01f, 1)] private float _gizmoHeight = 0.2f;

    [SerializeField] public TileType type;
    public List<Tile> neighbors;
    public float f;
    public float h;

    public static bool operator <(Tile a, Tile b)
        => (a.f < b.f);
    public static bool operator >(Tile a, Tile b)
        => (a.f > b.f);

    /*
    public List<KeyValuePair<float, Tile>> GetNeigbors() {
        List<KeyValuePair<float, Tile>> list = new List<KeyValuePair<float, Tile>>();
        for
        return list;
    }
    */

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        if (neighbors != null) {
            foreach (Tile t in neighbors) {
                Gizmos.DrawLine(transform.position + new Vector3(0, _gizmoHeight, 0), t.transform.position + new Vector3(0, _gizmoHeight, 0));
            }
        }
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + new Vector3(0, _gizmoHeight, 0), _gizmoSize);
    }

    public int CompareTo(Tile t) {
        if (t.f > f) return 1;
        if (t.f < f) return -1;
        else return 0;
    }
#endif
}
