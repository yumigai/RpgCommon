using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookToCameraMng : MonoBehaviour {

    [SerializeField]
    public Transform Target;

    private void Awake()
    {
        if(Target == null)
        {
            Target = Camera.main.transform;
        }
    }

    private void FixedUpdate()
    {
        this.transform.LookAt(Target);
    }
}
