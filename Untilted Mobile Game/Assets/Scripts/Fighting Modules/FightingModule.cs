using UnityEngine;

public class FightingModule : MonoBehaviour
{
    [SerializeField] private GameObject lockedPath;

    private int requiredEnemiesEliminated = 6;

    private bool spawnedEnemies;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !spawnedEnemies)
        {
            spawnedEnemies = true;
            GetComponentInChildren<EnemySpawner>().StartSpawn();
        }
    }
}
