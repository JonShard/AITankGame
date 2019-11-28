using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), SelectionBase]
public class Tank : MonoBehaviour
{
    [Header("Stats")]
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
    [Header("State")]
    public int lives = 1;
    public bool alive = true;
    [Header("Other")]
    public float turretEjectForce = 10;

    [HideInInspector] public float turning = 0;
    [HideInInspector] public float throttle = 0;
    [HideInInspector] public float turretMove = 0;

    Rigidbody rigid;
    Rigidbody bodyRigid;
    Rigidbody turretRigid;
    Transform barrelTip;
    ParticleSystem fireEffect;
    float _gunCooldown = -1;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        bodyRigid = transform.Find("Body").GetComponent<Rigidbody>();
        turretRigid = transform.Find("Turret").GetComponent<Rigidbody>();
        barrelTip = turretRigid.transform.Find("BarrelTip");
        fireEffect = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_gunCooldown > 0)
            _gunCooldown -= Time.deltaTime;

        float steeringPower = Mathf.Min((Vector3.Magnitude(rigid.velocity) / maxSpeed) + baseTurnPower, 1);
        float reverseSteering = (transform.worldToLocalMatrix * new Vector4(rigid.velocity.x, rigid.velocity.y, rigid.velocity.z, 0)).z;
        reverseSteering = (reverseSteering > -0.001f) ? 1 : -1;

        rigid.AddRelativeForce(new Vector3(0, 0, acceleration * throttle), ForceMode.Acceleration);
        rigid.AddRelativeTorque(new Vector3(0, reverseSteering * turnAcceleration * turning * steeringPower, 0));

        rigid.velocity = Vector3.ClampMagnitude(rigid.velocity, maxSpeed);
        rigid.angularVelocity = Vector3.ClampMagnitude(rigid.angularVelocity, turnMaxSpeed);

        turretRigid.AddRelativeTorque((new Vector3(0, turretMove * lookSpeed, 0)), ForceMode.Acceleration);

        turning = 0;
        throttle = 0;
        turretMove = 0;
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
        if (Physics.Raycast(barrelTip.position, barrelTip.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(barrelTip.position, barrelTip.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
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
            Debug.DrawRay(barrelTip.position, barrelTip.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }

        if (bulletPrefab) 
        {
            GameObject bullet = Instantiate(bulletPrefab, barrelTip.position, barrelTip.rotation);
            bullet.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, bulletSpeed), ForceMode.Impulse);
        }
        fireEffect.Clear();
        fireEffect.Play();
        Vector4 recoil = turretRigid.transform.worldToLocalMatrix * new Vector4(_recoilForce, 0, 0, 0);
        bodyRigid.AddRelativeTorque(recoil , ForceMode.Impulse);
    }

    [ContextMenu("RegisterHit")]
    public void RegisterHit()
    {
        lives--;
        if (lives <= 0)
        {
            Destroy(turretRigid.GetComponent<ConfigurableJoint>());
            turretRigid.AddRelativeForce(new Vector3(0, 1, 0.2f) * turretEjectForce, ForceMode.Impulse);
            turretRigid.AddRelativeTorque(Random.rotation.eulerAngles * turretEjectForce*20);
        }
    }
}
