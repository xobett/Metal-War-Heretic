using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour, IDragHandler
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
