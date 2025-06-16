using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    [Header("NAVIGATION AREA SETTINGS")]
    [SerializeField] private float areaRadius;
    [SerializeField] private Transform playerPos;

    [Header("ATTACKING ENEMIES")]
    [SerializeField] private List<EnemyBase> attackingEnemies = new List<EnemyBase>();

    private void Awake()
    {
        instance = this;
    }

    public int ActiveAttackingEnemiesCount => attackingEnemies.Count;

    public void AddAttackingEnemy(EnemyBase enemy)
    {
        attackingEnemies.Add(enemy);
    }

    public void RemoveAttackingEnemy(EnemyBase enemy)
    {
        attackingEnemies.Remove(enemy);
    }

    public Vector3 AssignMovingPosition()
    {
        Vector3 pos = Random.insideUnitSphere * areaRadius;
        pos.y = 0;
        pos = playerPos.position + pos;
        return pos;
    }

    public float AreaRadius => areaRadius;

    public void IncreaseAreaRadius()
    {
        areaRadius += 0.5f;
        Debug.Log("Area was increased");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        
        if (playerPos != null)
        {
            Gizmos.DrawWireSphere(playerPos.position, areaRadius);
        }
    }

}
