using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPickup : MonoBehaviour
{
    public GameObject bulletPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameManager.Instance.player.GetComponent<Player>().AddToClip(bulletPrefab);
            Destroy(this.gameObject);
        }
    }

}
