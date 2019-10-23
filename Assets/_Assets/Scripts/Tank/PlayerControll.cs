using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tank))]
public class PlayerControll : MonoBehaviour
{
    Tank tank;

    // Start is called before the first frame update
    void Start()
    {
        tank = GetComponent<Tank>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) tank.throttle = 1;
        else if (Input.GetKey(KeyCode.S)) tank.throttle = -1;
        else tank.throttle = 0;

        if (Input.GetKey(KeyCode.D)) tank.turning = 1;
        else if (Input.GetKey(KeyCode.A)) tank.turning = -1;
        else tank.turning = 0;

        tank.turretMove = Input.GetAxis("Mouse X");

        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) tank.FireGun();

    }
}
