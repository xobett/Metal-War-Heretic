using EnemyAI.BruteEnemy;
using EnemyAI.ElectricEnemy;
using UnityEngine;

public class EE_AnimEvents : MonoBehaviour
{
    private ElectricEnemy electricEnemy;

    [SerializeField] private GameObject electricAreaVfx;
    private Vector3 vfxPos;

    private void Start()
    {
        electricEnemy = GetComponentInParent<ElectricEnemy>();
        vfxPos = transform.parent.transform.GetChild(4).transform.position;
    }

    public void PlayElectricAreaAnticipation()
    {
        GameObject vfx = Instantiate(electricAreaVfx, vfxPos, electricAreaVfx.transform.rotation);
        Destroy(vfx, 3);
    }

    public void ThrowLightning()
    {
        electricEnemy.AnimEvent_ThrowLightning();
    }

    public void SpawnElectricArea()
    {
        electricEnemy.AnimEvent_SpawnElectricArea();
    }

    public void StopFacingAtPlayer()
    {
        electricEnemy.AnimEvent_StopFacingAtPlayer();
    }

    public void ResetRotation()
    {
        electricEnemy.AnimEvent_SmoothResetRotation();
    }

    public void FinishAttack()
    {
        electricEnemy.AnimEvent_FinishAttack();
    }

    public void Death()
    {
        electricEnemy.AnimEvent_OnDeath();
    }
}
