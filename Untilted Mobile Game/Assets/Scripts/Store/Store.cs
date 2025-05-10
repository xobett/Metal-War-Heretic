using UnityEngine;

public class Store : MonoBehaviour, IInteractable
{
    [SerializedField] private int itemCost;

    private void BuyItem() 
    {
        
    }

    public void OnInteract()
    {
        BuyItem();
    }
}

