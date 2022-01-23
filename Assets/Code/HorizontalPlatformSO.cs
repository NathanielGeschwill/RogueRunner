using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Horz Spawner", menuName = "Horizontal Spawner")]
public class HorizontalPlatformSO : ScriptableObject
{
    public float yLevel;
    public float distanceBetweenPlats;
    public float chanceToSpawn;
    public GameObject[] potentialPlatforms;
}
