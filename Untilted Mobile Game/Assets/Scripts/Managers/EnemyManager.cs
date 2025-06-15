using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("PLAYER DETECTION SETTINGS")]
    [SerializeField] private float waitPointAreaRadius;
    [SerializeField] private Transform playerPos;
    [SerializeField] private LayerMask whatIsPlayer;

    [SerializeField] private GameObject testObject;
    [SerializeField] private float testObjectRadius;

    void Start()
    {
        GenerateRandomPosition();
    }

    // Update is called once per frame
    void Update()
    {

    }

    [ContextMenu("Gemerate Position")]
    private void GenerateRandomPosition()
    {
        StartCoroutine(GetPos());
    }

    IEnumerator GetPos()
    {
        testObject = new();
        testObject.name = "Test";

        Vector3 pos = GetRandomPos();

        int attemptsDone = 0;
        while (Physics.CheckSphere(pos, testObjectRadius, whatIsPlayer))
        {
            if (attemptsDone >= 3)
            {
                waitPointAreaRadius += 0.5f;
                Debug.Log("Area was incremented");
            }

            pos = GetRandomPos();
            attemptsDone++;
            Debug.Log("New position was generated");
            yield return null;
        }

        testObject.transform.position = pos;
        yield return null;

    }

    private Vector3 GetRandomPos()
    {
        Vector3 pos = Random.insideUnitSphere * waitPointAreaRadius;
        pos.y = 0;
        pos = playerPos.position + pos;
        return pos;
    }

    private void OnDrawGizmos()
    {
        if (playerPos == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerPos.position, waitPointAreaRadius);

        if (testObject != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(testObject.transform.position, testObjectRadius);
        }

    }
}
