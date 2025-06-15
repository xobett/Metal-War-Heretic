using UnityEngine;

public class testRandomPos : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Starting pos: " + transform.position);
        Vector3 randomPos = Random.insideUnitSphere * 1;
        randomPos.y = 0;

        transform.position = transform.position + randomPos;
        Debug.Log("New pos: " + transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
