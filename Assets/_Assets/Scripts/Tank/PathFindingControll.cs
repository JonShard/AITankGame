using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tank))]
public class PathFindingControll : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField, Range(0.2f, 10)] private float _stopFollowingAtDistance = 1; 

    [Header("Stats")]
    [SerializeField, Range(1,10)] private float _fireRange = 3;

    [Header("Debug")]
    [SerializeField] private Transform _target;
    [SerializeField] private List<Transform> _path;
    [SerializeField, Range(0.2f, 2)] private float _raycastHeightOffset = 0.5f;
    [SerializeField] private bool _drawGizmos = false;

    Tank _tank;
    Rigidbody _rigid;
    PathFinder _pathFinder;

    // Start is called before the first frame update
    private void Start()
    {
        _tank = GetComponent<Tank>();
        _rigid = GetComponent<Rigidbody>(); // Required through Tank.
        _pathFinder = GameObject.Find("TileMap").GetComponent<PathFinder>();
        if (_pathFinder == null) 
        {
            Debug.LogWarning("No Gameobject with name TileMap with a PathFiner component");
            Destroy(this);
        }

        _path = _pathFinder.GetPath(transform, _target);
    }

    private bool RaycastTarget()
    {
        int layerMask = 1 << 17; // Add layer 17, TankHitbox to mask.
        layerMask = ~layerMask;

        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * _raycastHeightOffset;
        Vector3 direction = (_target.position - (transform.position + Vector3.up * _raycastHeightOffset)).normalized;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(origin, direction, out hit, _fireRange, layerMask))
        {

            if (hit.transform.root.GetComponent<PlayerControll>() != null)
            { 
                Debug.DrawRay(origin, direction * hit.distance, Color.red);
                Debug.Log("Raycast the tank!");
                return true;
            }
        }

        Debug.DrawRay(origin, direction * _fireRange, Color.white);
        return false;
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_path == null || _target == null)
            return;

        if (RaycastTarget() )
        {
            _tank.FireGun();
        }

        if(_path.Count > 0)
        {
            if (Vector3.SqrMagnitude(_path[0].position - transform.position) < 0.05f)
            {
                Debug.Log("AI: Reached a node " + _path[_path.Count - 1].name + ", moving to next.");
                _path.RemoveAt(0);
            }
            _rigid.AddForce((_path[0].position - transform.position).normalized * _tank.acceleration, ForceMode.Acceleration);
        }

        if (_path.Count == 0)
        {
            if ((_target.position - transform.position).magnitude > _fireRange)
            {
                _rigid.AddForce((_target.position - transform.position).normalized * _tank.acceleration, ForceMode.Acceleration);
            }
            else
            {
                Debug.Log("AI: Fire at player tank!!");
                _tank.FireGun();
            }
        }


        _rigid.velocity = Vector3.ClampMagnitude(_rigid.velocity, _tank.maxSpeed);
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_drawGizmos && _path != null && _path.Count > 0)
        {
            Gizmos.color = new Color(15, 91, 255);
            Gizmos.DrawLine(transform.position, _path[0].position);
            for (int i = 1; i < _path.Count; i++)
            {
                if (_path[i - 1] != null && _path[i] != null)
                Gizmos.DrawLine(_path[i - 1].position, _path[i].position);
            }
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _fireRange);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _stopFollowingAtDistance);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + Vector3.up * _raycastHeightOffset, 0.06f);
    }
#endif

}
