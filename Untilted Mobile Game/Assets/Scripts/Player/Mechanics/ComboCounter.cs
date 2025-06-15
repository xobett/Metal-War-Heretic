using System.Collections;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

public class ComboCounter : MonoBehaviour
{
    [Header("ACTUAL COMBO COUNT")]
    [SerializeField] private int comboCount;
    [SerializeField] private TextMeshProUGUI hudComboText;

    [Header("COMBO TIMER SETTINGS")]
    [SerializeField] private float timeWithinCombo;

    private float timer;
    private const float originalTimeWindow = 2f;

    private void Update()
    {
        DisplayComboCounter();
        ComboCheck();
        RunningTimer();
    }

    #region COMBO COUNTER

    private void DisplayComboCounter()
    {
        hudComboText.text = $"{comboCount}X";
    }

    private void ComboCheck()
    {
        if (IsHitting())
        {
            comboCount++;
            timeWithinCombo -= timeWithinCombo <= 1 ? 0 : 0.1f;
            timer = timeWithinCombo;

        }

        if (timer < 0)
        {
            comboCount = 0;
            timeWithinCombo = originalTimeWindow;
        }
    }

    #endregion COMBO COUNTER

    #region COMBO TIMER

    private void RunningTimer()
    {
        timer -= Time.deltaTime;
    }

    #endregion COMBO TIMER


    public bool IsHitting() => Input.GetKeyDown(KeyCode.E);
}