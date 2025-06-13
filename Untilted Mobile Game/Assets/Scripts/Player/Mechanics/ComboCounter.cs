using System.Collections;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

public class ComboCounter : MonoBehaviour
{
    //WHENEVER A HIT IS LANDED AND IS BETWEEN THE CURRENT TIME OF COMBO EXECUTION, IT WILL ADD
    //UPDATE VISUALLY TO THE PLAYER THE COMBO COUNTER

    [SerializeField] private int comboCount;

    [SerializeField] private TextMeshProUGUI comboText;

    private bool timerEnabled;

    [SerializeField] private float timer;
    [SerializeField] private float timeWithinCombo;
    private const float originalTimeWindow = 2f;

    private void Start()
    {
        timerEnabled = false;
    }

    private void Update()
    {
        DisplayComboCounter();
        ComboCheck();
        RunningTimer();
    }

    #region COMBO COUNTER

    private void DisplayComboCounter()
    {
        comboText.text = $"{comboCount}X";
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