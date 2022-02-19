using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    private Transform FloatingLocal;
    public float freq= 4;
    public float amp = 1.2f;
   
    void Start()
    {
        FloatingLocal = transform;
    }
 
    void FixedUpdate()
    {
        transform.position = FloatingLocal.position + new Vector3(0, Mathf.Sin(Time.time * freq) * Time.deltaTime * amp , 0);
        //gameObject.transform.rotation = new Vector3()
    }
}
