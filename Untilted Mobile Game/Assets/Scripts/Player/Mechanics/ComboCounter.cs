using TMPro;
using UnityEngine;

public class ComboCounter : MonoBehaviour
{
    [Header("ACTUAL COMBO COUNT")]
    [HideInInspector] public int ActualComboCount { get; private set; }

    [Header("COMBO TIMER SETTINGS")]
    [SerializeField] private float timeWithinCombo;

    private float timer;
    private const float originalTimeWindow = 2f;

    private void Update()
    {
        HandleComboReset();
        RunningTimer();
    }

    #region COMBO COUNTER

    private void HandleComboReset()
    {
        if (timer < 0)
        {
            ActualComboCount = 0;
            timeWithinCombo = originalTimeWindow;
        }
    }

    public void IncreaseComboCount()
    {
        ActualComboCount += 2;
        timeWithinCombo -= timeWithinCombo <= 1 ? 0 : 0.1f;
        timer = timeWithinCombo;
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