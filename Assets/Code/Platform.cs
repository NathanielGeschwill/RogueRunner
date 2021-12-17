using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private BoxCollider bc;

    private void OnEnable()
    {
        Player.OnJump += DisableCollider;
        Player.OnFall += EnableCollider;
    }

    private void OnDisable()
    {
        Player.OnJump -= DisableCollider;
        Player.OnFall -= EnableCollider;
    }

    // Start is called before the first frame update
    void Start()
    {
        bc = GetComponent<BoxCollider>();
    }

    private void DisableCollider()
    {
        bc.enabled = false;
    }

    private void EnableCollider()
    {
        bc.enabled = true;
    }
}
