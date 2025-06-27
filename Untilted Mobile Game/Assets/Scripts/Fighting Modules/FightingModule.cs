using UnityEngine;
using System.Collections.Generic;

public class FightingModule : MonoBehaviour
{
    [Header("FIGHTING MODULE SETTINGS\n")]

    [Header("LOCKED PATH")]
    [SerializeField] private GameObject lockedPath;

    [Header("MODULE ENEMIES")]
    List<GameObject> moduleEnemies = new List<GameObject>();

    //Will be used later for spawn upon entering area

    //private bool spawnedEnemies;
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player") && !spawnedEnemies)
    //    {
    //        spawnedEnemies = true;
    //        GetComponentInChildren<EnemySpawner>().StartSpawn();
    //    }
    //}
}
