using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Tank), typeof(Animator))] // typeof(StateMachineControl)
public class StateMachine : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField, Tooltip("How many tiles should the tank remember that it's been on, so it doesn't walk in very small circles.")]
    int _recentListLength = 10;
    [SerializeField]
    float _visualRange = 6;
    [SerializeField]
    float _fireRange = 3.5f;
    
    [Header("State")]
    public List<Tile> recentTiles;
    public List<Tank> enemies;
    public Tank target;
    public Transform lastKnowPos;

    [Header("State Machine Parameters")]    // We feed these into the animator.
    public bool hasTarget = false;
    public bool canFire = false;

    [Header("Raycast")]
    [SerializeField, Range(0.2f, 2)] private float _raycastHeightOffset = 0.5f;
    [SerializeField] private bool _drawGizmos = false;

    List<Tank> lineOfSight = new List<Tank>();
    List<float> distances = new List<float>();

    Animator animator;              // Makes decisions on what to do based on parameters we give it.
    //StateMachineControl control;    // Moves thew tank around.

    // Start is called before the first frame update
    void Start()
    {
        recentTiles = new List<Tile>();
        enemies = new List<Tank>(FindObjectsOfType<Tank>());    // Find all tanks in scene.
        enemies.Remove(GetComponent<Tank>());                   // Remove ourselves from enemy list.

        animator = GetComponent<Animator>();
        //control = GetComponent<StateMachineControl>();
    }

    // Retuns distance to t if tank has line of sight to t, -1 otherwise.
    private float RaycastTarget(Tank t, float range, Color lineColor)
    {
        int layerMask = 1 << 17; // Add layer 17, TankHitbox to mask.
        layerMask = ~layerMask;

        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * _raycastHeightOffset;
        Vector3 direction = (t.transform.position - (transform.position + Vector3.up * _raycastHeightOffset)).normalized;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(origin, direction, out hit, range, layerMask))
        {
            if (hit.transform.root.GetComponent<Tank>() != null)
            {
                Debug.DrawRay(origin, direction * hit.distance, lineColor);
                return hit.distance;
            }
        }
        Debug.DrawRay(origin, direction * range, Color.white);
        return -1;  // No line of sight.
    }

    void UpdateStateParameters()
    {
        hasTarget = (target);   // True of target is not null.

        if (target && 
            lineOfSight.Contains(target) &&
            distances[lineOfSight.IndexOf(target)] < _fireRange)
        {
            canFire = true;
        }
        else
        {
            canFire = false;
        }

        // Tell the animator what the state is so it can decide what to do next.
        animator.SetBool("hasTarget", hasTarget);
        animator.SetBool("canFire", canFire);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Update internal state vaiables, like who our target is and rather or not we can see it.
        lineOfSight.Clear();
        distances.Clear();
        foreach (Tank t in enemies)
        {
            if (t.lives <= 0)      // If an enemy has died, we remove it and consider next tank.
            {
                enemies.Remove(t);
                continue;
            }
            float dist = RaycastTarget(t, _visualRange, Color.yellow);
            if (dist > 0) // Do we see the tank?
            {
                lineOfSight.Add(t);
                distances.Add(dist);
            }
        }
        if (distances.Count > 0)    // Update who our target is.
        {
            int closestVisibleTank = distances.IndexOf(distances.Min());
            target = lineOfSight[closestVisibleTank];
        }
        
        // Tell the state machine what's happening. 
        UpdateStateParameters();
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_drawGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _visualRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _fireRange);
            Gizmos.DrawSphere(transform.position + Vector3.up * _raycastHeightOffset, 0.06f);
        }
    }
#endif
}
