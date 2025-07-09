using UnityEngine;

public class SE_AnimEvents : MonoBehaviour
{
    [SerializeField] Collider[] hitColliders;
    public void EnablePushAttack()
    {
        foreach (Collider collider in hitColliders)
        {
            collider.isTrigger = true;
        }

        GetComponentInParent<ShieldEnemy>().rgIsPushing = true;
    }

    public void DisablePushAttack()
    {

        foreach (Collider collider in hitColliders)
        {
            collider.isTrigger = false;
        }

        GetComponentInParent<ShieldEnemy>().rgIsPushing = false;
    }
}
