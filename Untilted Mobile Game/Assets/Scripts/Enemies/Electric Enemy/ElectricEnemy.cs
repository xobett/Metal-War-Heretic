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

        [SerializeField] private float beforeDistanceAttackTime;
        [SerializeField] private float afterDistanceAttackTime;

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

        #region MAIN ABILITY - DISTANCE ATTACK

        private void DistanceAttack()
        {
            animator.SetTrigger("DistanceAttack");
        }

        public void ThrowElectricBall()
        {
            distanceAttackPf.GetComponent<ElectricBall>().SetElectricBallSettings(transform.forward, distanceAttackDamage, distanceAttackSpeed, distanceAttackLifetime);
            Instantiate(distanceAttackPf, spawnPoint.position, Quaternion.identity);
        }

        #endregion MAIN ABILITY - DISTANCE ATTACK

        #region SECONDARY ABILITY - ELECTRIC ATTACK

        private void ElectricAttack()
        {
            animator.SetTrigger("ElectricAttack");
            electricAttackCoolingDown = true;
        }

        public void SpawnElectricVFX()
        {
            electricAreaSpawnPos = player.transform.position;
            electricAreaSpawnPos.y = 1.2f;

            GameObject vfx = Instantiate(cueElectricVfx, electricAreaSpawnPos, cueElectricVfx.transform.rotation);
            Destroy(vfx, 3);

            Invoke(nameof(SpawnElectricArea), 3f);
        }

        private void SpawnElectricArea()
        {
            electricAreaPf.GetComponent<ElectricArea>().SetElectricAreaSettings(electricAreaDamage, electricAreaLifetime);
            Instantiate(electricAreaPf, electricAreaSpawnPos, electricAreaPf.transform.rotation);
            StartCoroutine(StartElectricCooldown());
        }

        private IEnumerator StartElectricCooldown()
        {
            yield return new WaitForSeconds(electricAttackCooldownTime);

            electricAttackCoolingDown = false;
            yield return null;
        }

        #endregion SECONDARY ABILITY - ELECTRIC ATTACK

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.IncreaseScore(50);
            }
        }
    }
}

