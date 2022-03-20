using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    private Transform FloatingLocal;
    public float freq= 4;
    public float amp = 1.2f;
    public Vector3 _rotation;
   
    void Start()
    {
        FloatingLocal = transform;
    }
 
    void FixedUpdate()
    {
        transform.position = FloatingLocal.position + new Vector3(0, Mathf.Sin(Time.time * freq) * Time.deltaTime * amp , 0);
        transform.Rotate(_rotation * Time.deltaTime);
    }
}
