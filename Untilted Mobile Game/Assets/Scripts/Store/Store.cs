using UnityEngine;

public class Store : MonoBehaviour, IInteractable
{
    [SerializeField] private int itemCost;

    private void BuyItem() 
    {
        Debug.Log("Is interacting with store");
    }

    public void OnInteract()
    {
        BuyItem();
    }
}

