using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tank))]
public class StateMachineControl : MonoBehaviour
{
    [SerializeField] private bool _debug = false;
    public Transform aimAtTrasform;

    public List<Transform> goToList = new List<Transform>();

    Tank _tank;
    Rigidbody _rigid;

    // Start is called before the first frame update
    void Start()
    {
        _tank = GetComponent<Tank>();
        _rigid = GetComponent<Rigidbody>(); // Required through Tank.
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (goToList.Count > 0)
        {
            if (Vector3.SqrMagnitude(goToList[0].position - transform.position) < 0.05f)
            {
                if (_debug) Debug.Log("AI: Reached a node " + goToList[goToList.Count - 1].name + ", moving to next.");
                goToList.RemoveAt(0);
            }
            else
            {
                _rigid.AddForce((goToList[0].position - transform.position).normalized * _tank.acceleration, ForceMode.Acceleration);
            }
        }
        _rigid.velocity = Vector3.ClampMagnitude(_rigid.velocity, _tank.maxSpeed);

        if (aimAtTrasform)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(aimAtTrasform.position - transform.position, transform.up), _tank.lookSpeed * 50 * Time.deltaTime);
        }


    }
}
