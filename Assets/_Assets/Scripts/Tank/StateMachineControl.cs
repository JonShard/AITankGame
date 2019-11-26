using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tank))]
public class StateMachineControl : MonoBehaviour
{
    public Transform aimAtTrasform;
    public Transform moveToTrasform;

    Tank tank; 

    // Start is called before the first frame update
    void Start()
    {
        tank = GetComponent<Tank>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (move to transform
        // if reached set as null.
            
        // if aim at thing
    }
}
