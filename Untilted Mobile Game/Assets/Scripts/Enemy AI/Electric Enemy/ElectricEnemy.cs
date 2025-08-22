using UnityEngine;

namespace EnemyAI.ElectricEnemy
{
    public class ElectricEnemy : Enemy
    {
        [Header("--- ELECTRIC ENEMY SETTINGS ---\n")]

        [Header("MAIN ABILITY - DISTANCE ATTACK\n")]

        [Header("DISTANCE ATTACK PREFAB")]
        [SerializeField] private GameObject distanceAttackPf;

        [Header("DISTANCE ATTACK SETTINGS")]
        [SerializeField] private Transform spawnPoint;

        [SerializeField] private GameObject lightningVfx; 

        [SerializeField] private float distanceAttackSpeed;
        [SerializeField] private float distanceAttackDamage;
        [SerializeField] private float distanceAttackLifetime;

        [Header("SECONDARY ABILITY - ELECTRIC ATTACK\n")]

        [Header("ELECTRIC ATTACK PREFAB")]
        [SerializeField] private GameObject electricAreaPf;

        [Header("ELECTRIC ATTACK SETTINGS")]
        [SerializeField] private int electricAreaLifetime;
        [SerializeField] private int electricAreaDamage;

        private Vector3 electricAreaSpawnPos;

        [SerializeField, Range(1f, 55f)] private int electricAttackCooldownTime;
        [SerializeField] private bool electricAttackCoolingDown;

        public override void Attack()
        {
            GetAttackAbility();
        }

        private void GetAttackAbility()
        {
            if (!electricAttackCoolingDown)
            {
                ExecuteElectricAttack();
            }
            else
            {
                ExecuteDistanceAttack();
            }
        }

        #region DISTANCE ATTACK

        private void ExecuteDistanceAttack()
        {
            animator.SetTrigger("DistanceAttack");
        }

        #endregion DISTANCE ATTACK

        #region ELECTRIC ATTACK
        private void ExecuteElectricAttack()
        {
            electricAttackCoolingDown = true;
            animator.SetTrigger("ElectricAttack");
        }

        private void RunCooldown()
        {
            Invoke(nameof(DisableCooldownPH), electricAttackCooldownTime);
        }

        private void DisableCooldownPH()
        {
            electricAttackCoolingDown = false;
        }

        #endregion ELECTRIC ATTACK

        #region Animation Event Methods

        public void AnimEvent_ThrowLightning()
        {
            Vector3 spawnPos = spawnPoint.position;
            spawnPos.y = 1.5f;
            GameObject lightning = Instantiate(distanceAttackPf, spawnPos, Quaternion.identity);
            lightning.GetComponent<LightningCollider>().SetLightningSettings(transform.forward, distanceAttackDamage, distanceAttackSpeed);

            Quaternion rot = Quaternion.Euler(-90, spawnPoint.transform.eulerAngles.y, spawnPoint.eulerAngles.z);
            GameObject vfx = Instantiate(lightningVfx, spawnPoint.position, rot);
            Destroy(vfx, 3);

            audioSource.clip = AudioManager.Instance.GetClip("ATAQUE ELECTRICIDAD SUELO");
            audioSource.Play();
        }

        public void AnimEvent_SpawnElectricArea()
        {
            electricAreaSpawnPos = player.transform.position;
            electricAreaSpawnPos.y = 0.8f;

            electricAreaPf.GetComponent<ElectricArea>().SetElectricAreaSettings(electricAreaDamage, electricAreaLifetime);
            Instantiate(electricAreaPf, electricAreaSpawnPos, Quaternion.identity);
            RunCooldown();
        }

        #endregion Animation Event Methods

    }
}

