using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), SelectionBase]
public class Tank : MonoBehaviour
{

    public enum MovementScheme
    {
        WaypointList, UserInput, ControlValues
    }

    [SerializeField] private bool _debug = false;
    [SerializeField] private bool _drawGizmos = false;
    [Header("Stats")]
    public MovementScheme movementScheme = MovementScheme.WaypointList;
    public float acceleration = 3;
    public float maxSpeed = 2;
    public float turnAcceleration = 1;
    public float turnMaxSpeed = 1;
    [Range(0, 0.5f)]public float baseTurnPower = 0.04f;
    public float lookSpeed = 10;
    public int maxLives = 1;
    [Header("Gun")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 50;
    [Range(0.2f, 8)] public float reloadTime = 4;
    [Range(0, 100)] public float _recoilForce = 30;
    [Header("Other")]
    public float turretEjectForce = 10;
    [Header("State")]
    public int lives = 1;
    public bool alive = true;
    public Transform aimAtTrasform;
    public List<Transform> waypointList = new List<Transform>();

    [HideInInspector] public float turning = 0;
    [HideInInspector] public float throttle = 0;
    [HideInInspector] public float turretMove = 0;

    Rigidbody _rigid;
    Rigidbody _bodyRigid;
    Rigidbody _turretRigid;
    Transform _barrelTip;
    ParticleSystem _fireEffect;
    float _gunCooldown = -1;

    PathFinder _pathFinder;
    public List<Transform> _path;

    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody>();
        _bodyRigid = transform.Find("Body").GetComponent<Rigidbody>();
        _turretRigid = transform.Find("Turret").GetComponent<Rigidbody>();
        _barrelTip = _turretRigid.transform.Find("BarrelTip");
        _fireEffect = GetComponentInChildren<ParticleSystem>();

        _pathFinder = GameObject.Find("TileMap").GetComponent<PathFinder>();
        if (_pathFinder == null)
        {
            Debug.LogWarning("No Gameobject with name TileMap with a PathFiner component");
            Destroy(this);
        }
    }

    private void MoveUsingList()
    {
        if (waypointList.Count > 0)
        {
            if (Vector3.SqrMagnitude(waypointList[0].position - transform.position) < 0.05f)
            {
                if (_debug) Debug.Log("AI: Reached a node " + waypointList[waypointList.Count - 1].name + ", moving to next.");
                waypointList.RemoveAt(0);
            }
            else
            {
                _rigid.AddForce((waypointList[0].position - transform.position).normalized * acceleration, ForceMode.Acceleration);
            }
        }
        _rigid.velocity = Vector3.ClampMagnitude(_rigid.velocity, maxSpeed);

        if (aimAtTrasform)
        {
            _turretRigid.rotation = Quaternion.RotateTowards(_turretRigid.rotation, Quaternion.LookRotation(aimAtTrasform.position - _turretRigid.position, transform.up), lookSpeed * 50 * Time.deltaTime);
        }
    }

    private void MoveUsingUserInput()
    {
        if (Input.GetKey(KeyCode.W)) throttle = 1;
        else if (Input.GetKey(KeyCode.S)) throttle = -1;
        else throttle = 0;

        if (Input.GetKey(KeyCode.D)) turning = 1;
        else if (Input.GetKey(KeyCode.A)) turning = -1;
        else turning = 0;

        turretMove = Input.GetAxis("Mouse X");

        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
            FireGun();

        MoveUsingValues();
    }

    private void MoveUsingValues()
    {
        float steeringPower = Mathf.Min((Vector3.Magnitude(_rigid.velocity) / maxSpeed) + baseTurnPower, 1);
        float reverseSteering = (transform.worldToLocalMatrix * new Vector4(_rigid.velocity.x, _rigid.velocity.y, _rigid.velocity.z, 0)).z;
        reverseSteering = (reverseSteering > -0.001f) ? 1 : -1;

        _rigid.AddRelativeForce(new Vector3(0, 0, acceleration * throttle), ForceMode.Acceleration);
        _rigid.AddRelativeTorque(new Vector3(0, reverseSteering * turnAcceleration * turning * steeringPower, 0));
        _turretRigid.AddRelativeTorque((new Vector3(0, turretMove * lookSpeed, 0)), ForceMode.Acceleration);
        turning = 0;
        throttle = 0;
        turretMove = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_gunCooldown > 0)
            _gunCooldown -= Time.deltaTime;

        switch (movementScheme)
        {
            case MovementScheme.WaypointList:
                MoveUsingList();
                break;
            case MovementScheme.UserInput:
                MoveUsingUserInput();
                break;
            case MovementScheme.ControlValues:
                MoveUsingValues();
                break;
        }

        _rigid.velocity = Vector3.ClampMagnitude(_rigid.velocity, maxSpeed);
        _rigid.angularVelocity = Vector3.ClampMagnitude(_rigid.angularVelocity, turnMaxSpeed);
    }

    public void FireGun() 
    {
        if (_gunCooldown > 0)
            return;

        _gunCooldown = reloadTime;

        int tankBodyLayer = 1 << 17;    // Add layer 17, TankHitbox to mask.
        int defaultLayer = 1;           // Add default layer like walls to mask.
        int layerMask = tankBodyLayer & defaultLayer;
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(_barrelTip.position, _barrelTip.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(_barrelTip.position, _barrelTip.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Tank hitTank = hit.transform.GetComponent<Tank>();
            if(hitTank) 
            {
                Debug.Log("Hit a tank.");
                hitTank.RegisterHit();
            }
            else 
            {
                Debug.Log("Hit something.");
            }
        }
        else 
        {
            Debug.DrawRay(_barrelTip.position, _barrelTip.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }

        if (bulletPrefab) 
        {
            GameObject bullet = Instantiate(bulletPrefab, _barrelTip.position, _barrelTip.rotation);
            bullet.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, bulletSpeed), ForceMode.Impulse);
        }
        _fireEffect.Clear();
        _fireEffect.Play();
        Vector4 recoil = _turretRigid.transform.worldToLocalMatrix * new Vector4(_recoilForce, 0, 0, 0);
        _bodyRigid.AddRelativeTorque(recoil , ForceMode.Impulse);
    }

    [ContextMenu("RegisterHit")]
    public void RegisterHit()
    {
        lives--;
        if (lives <= 0)
        {
            Destroy(_turretRigid.GetComponent<ConfigurableJoint>());
            _turretRigid.AddRelativeForce(new Vector3(0, 1, 0.2f) * turretEjectForce, ForceMode.Impulse);
            _turretRigid.AddRelativeTorque(Random.rotation.eulerAngles * turretEjectForce*20);
        }
    }

    public void UpdatePath()
    {
        Debug.Log("Works");
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_drawGizmos && waypointList != null && waypointList.Count > 0)
        {
            Gizmos.color = new Color(15, 91, 255);
            Gizmos.DrawLine(transform.position, waypointList[0].position);
            for (int i = 1; i < waypointList.Count; i++)
            {
                if (waypointList[i - 1] != null && waypointList[i] != null)
                    Gizmos.DrawLine(waypointList[i - 1].position, waypointList[i].position);
            }
        }
    }
#endif
}
