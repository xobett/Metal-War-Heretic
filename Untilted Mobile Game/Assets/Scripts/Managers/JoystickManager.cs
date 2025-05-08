using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickManager : MonoBehaviour, IDragHandler
{
    //Joystick Images
    [SerializeField] private Image joystick_Handler;
    [SerializeField] private Image joystick_HandlerBckgrnd;
    [SerializeField] private Image joystick_Bckgrnd;

    //Input
    [SerializeField] private Vector2 posInput;

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        //Whenever there is an event where something is dragged, the IDragHandler sends to this method an event data containing data about the drag interaction.
        //Within that drag interaction, then using the following method, detects if a player touched from the screen to a point within the rect transform, needing the 
        //following parameters to provide the input details.
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystick_Bckgrnd.rectTransform, eventData.position, eventData.pressEventCamera, out posInput))
        {
            Debug.Log(posInput);
        }
    }
}