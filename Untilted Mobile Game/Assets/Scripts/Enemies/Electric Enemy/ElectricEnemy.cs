using System.Collections;
using UnityEngine;

namespace EnemyAI.ElectricEnemy
{
    public class ElectricEnemy : EnemyBase
    {
        [Header("--- ELECTRIC ENEMY SETTINGS ---\n")]

        [Header("MAIN ABILITY - DISTANCE ATTACK\n")]

        [Header("DISTANCE ATTACK PREFAB")]
        [SerializeField] private GameObject distanceAttackPf;

        [Header("DISTANCE ATTACK SETTINGS")]
        [SerializeField] private Transform spawnPoint;

        [SerializeField] private float distanceAttackSpeed;
        [SerializeField] private float distanceAttackDamage;
        [SerializeField] private float distanceAttackLifetime;

        [Header("SECONDARY ABILITY - ELECTRIC ATTACK\n")]

        [Header("ELECTRIC ATTACK PREFAB | CUE VFX")]
        [SerializeField] private GameObject electricAreaPf;
        [SerializeField] private GameObject cueElectricVfx;

        [Header("ELECTRIC ATTACK SETTINGS")]
        [SerializeField] private int electricAreaLifetime;
        [SerializeField] private int electricAreaDamage;

        private Vector3 electricAreaSpawnPos;

        [SerializeField, Range(1f, 10f)] private int electricAttackCooldownTime;
        private bool electricAttackCoolingDown;

        protected override void Attack()
        {
            GetAttackAbility();
        }

        private void GetAttackAbility()
        {
            if (!electricAttackCoolingDown)
            {
                ElectricAttack();
            }
            else
            {
                DistanceAttack();
            }
        }

        private void DistanceAttack()
        {
            animator.SetTrigger("DistanceAttack");
        }
        private void ElectricAttack()
        {
            electricAttackCoolingDown = true;
            animator.SetTrigger("ElectricAttack");
        }

        #region Animation Event Methods

        public void AnimEvent_ThrowElectricBall()
        {
            distanceAttackPf.GetComponent<ElectricBall>().SetElectricBallSettings(transform.forward, distanceAttackDamage, distanceAttackSpeed, distanceAttackLifetime);
            Instantiate(distanceAttackPf, spawnPoint.position, Quaternion.identity);
        }

        public void AnimEvent_SpawnElectricVFX()
        {
            electricAreaSpawnPos = player.transform.position;
            electricAreaSpawnPos.y = 1.2f;

            GameObject vfx = Instantiate(cueElectricVfx, electricAreaSpawnPos, cueElectricVfx.transform.rotation);
            Destroy(vfx, 1.6f);
        }

        public void AnimEvent_SpawnElectricArea()
        {
            electricAreaPf.GetComponent<ElectricArea>().SetElectricAreaSettings(electricAreaDamage, electricAreaLifetime);
            Instantiate(electricAreaPf, electricAreaSpawnPos, electricAreaPf.transform.rotation);
            RunCooldown();
        }

        #endregion Animation Event Methods

        private void RunCooldown()
        {
            Invoke(nameof(DisableCooldown), electricAttackCooldownTime);
        }

        private void DisableCooldown()
        {
            electricAttackCoolingDown = false;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.IncreaseScore(50);
            }
        }
    }
}

