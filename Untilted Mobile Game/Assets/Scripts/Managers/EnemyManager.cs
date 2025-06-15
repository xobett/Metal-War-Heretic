using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    [Header("PLAYER DETECTION SETTINGS")]
    [SerializeField] private float waitPointAreaRadius;
    [SerializeField] private Transform playerPos;
    [SerializeField] private LayerMask whatIsPlayer;

    [SerializeField] private GameObject testObject;
    [SerializeField] private float testObjectRadius;

    [Header("ATTACKING ENEMIES")]
    [SerializeField] private List<EnemyBase> attackingEnemies = new List<EnemyBase>();

    private Vector3 randomPos = Vector3.zero;

    private void Awake()
    {
        instance = this;
    }

    public int ActiveAttackingEnemies()
    {
        return attackingEnemies.Count;
    }

    public void AddAttackingEnemy(EnemyBase enemy)
    {
        attackingEnemies.Add(enemy);
    }

    public void RemoveAttackingEnemy(EnemyBase enemy)
    {
        attackingEnemies.Remove(enemy);
    }

}
