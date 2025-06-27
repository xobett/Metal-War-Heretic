using UnityEngine;

public class ShowEnemyInfo : MonoBehaviour
{
    [SerializeField] private GameObject[] enemiesInfo;
    [SerializeField] private GameObject enemyInfoToShow;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        DisplayEnemyInfo();
    }

    private void DisplayEnemyInfo()
    {
        //Hide all enemies info
        foreach (var enemy in enemiesInfo)
        {
            enemy.SetActive(false);
        }

        enemyInfoToShow.SetActive(true);
    }
}
