using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
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
    [Range(0, 100)] public float _recoilForce = 30;
    [Header("State")]
    public int lives = 1;
    public bool alive = true;

    [HideInInspector] public float turning = 0;
    [HideInInspector] public float throttle = 0;
    [HideInInspector] public float turretMove = 0;

    Rigidbody rigid;
    Rigidbody bodyRigid;
    Rigidbody turret;
    Transform barrelTip;
    ParticleSystem fireEffect;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        bodyRigid = transform.Find("Body").GetComponent<Rigidbody>();
        turret = transform.Find("Turret").GetComponent<Rigidbody>();
        barrelTip = turret.transform.Find("BarrelTip");
        fireEffect = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float steeringPower = Mathf.Min((Vector3.Magnitude(rigid.velocity) / maxSpeed) + baseTurnPower, 1);
        float reverseSteering = (transform.worldToLocalMatrix * new Vector4(rigid.velocity.x, rigid.velocity.y, rigid.velocity.z, 0)).z;
        reverseSteering = (reverseSteering > -0.001f) ? 1 : -1;

        rigid.AddRelativeForce(new Vector3(0, 0, acceleration * throttle), ForceMode.Acceleration);
        rigid.AddRelativeTorque(new Vector3(0, reverseSteering * turnAcceleration * turning * steeringPower, 0));

        rigid.velocity = Vector3.ClampMagnitude(rigid.velocity, maxSpeed);
        rigid.angularVelocity = Vector3.ClampMagnitude(rigid.angularVelocity, turnMaxSpeed);

        turret.AddRelativeTorque((new Vector3(0, turretMove * lookSpeed, 0)), ForceMode.Acceleration);

        turning = 0;
        throttle = 0;
        turretMove = 0;
    }

    public void FireGun() 
    {
        int tankBodyLayer = 1 << 16;
        int defaultLayer = 1;
        int layerMask = tankBodyLayer & defaultLayer;
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(barrelTip.position, barrelTip.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask)) {
            Debug.DrawRay(barrelTip.position, barrelTip.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
        else {
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
        Vector4 recoil = turret.transform.worldToLocalMatrix * new Vector4(_recoilForce, 0, 0, 0);
        bodyRigid.AddRelativeTorque(recoil , ForceMode.Impulse);
    }

    public void RegisterHit()
    {
        lives--;
        if (lives <= 0)
        {
            // Pop off turret.
        }
    }
}
