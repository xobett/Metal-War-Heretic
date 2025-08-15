using TMPro;
using UnityEngine;

public class HUDStatsDisplay : MonoBehaviour
{
    [Header("HUD TEXT SETTINGS")]
    [SerializeField] private TextMeshProUGUI comboCountText;

    private ComboCounter comboCounter;


    void Start()
    {
        comboCounter = GameObject.FindGameObjectWithTag("Player").GetComponent<ComboCounter>();
    }

    // Update is called once per frame
    void Update()
    {
        DisplayActualComboCount();
    }

    private void DisplayActualComboCount()
    {
        comboCountText.text = $"{comboCounter.ActualComboCount.ToString()}X";
    }
}
