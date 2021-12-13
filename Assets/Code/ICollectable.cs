using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICollectable : MonoBehaviour
{
    public GameObject collectPrefab;
    public delegate void Collected(string collectedItem);
    public static event Collected OnCollected;
    public string colletedItem;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            OnCollected?.Invoke(colletedItem);
            Destroy(gameObject);
        }
    }


}
