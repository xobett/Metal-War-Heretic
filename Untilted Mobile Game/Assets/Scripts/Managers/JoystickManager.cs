
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoystickManager : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public static JoystickManager Instance;

    //Joystick Images & Params
    [SerializeField] private Image joystick_Handler;
    [SerializeField] private Image joystick_Bckgrnd;

    //Input
    private Vector2 posInput;
    private Vector2 inputAreaSize;

    //Joystick drag settings
    private const float dragLimit = 6f;

    private void Start()
    {
        GetClickableArea();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (PauseManager.Instance.GamePaused) return;

        //Whenever there is an event where something is dragged, the IDragHandler sends to this method an event data containing data about the drag interaction.
        //Within that drag interaction, then using the following method, detects if a player touched from the screen to a point within the rect transform, needing the 
        //following parameters to provide the input details.
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystick_Bckgrnd.rectTransform, eventData.position, eventData.pressEventCamera, out posInput))
        {
            //Divides the pointer input position by the clickable area size
            posInput.x /= inputAreaSize.x;
            posInput.y /= inputAreaSize.y;

            //To avoid joystick handler from exits the clickable area, normalizes the vector
            if (posInput.magnitude > 1)
            {
                posInput = posInput.normalized;
            }

            //Moves the joystick handler in relation to its anchored position, with the position input multiplyed by the clickable area size.
            //Then, it limits its movement dividing it by a fixed value, to stay within the background.
            //Finally, by normalizing the vector, it stops it from moving gradually if the drag is moving way out of the clickable area.
            joystick_Handler.rectTransform.anchoredPosition = new Vector2(
                posInput.x * (inputAreaSize.x / dragLimit),
                posInput.y * (inputAreaSize.y / dragLimit));
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        joystick_Bckgrnd.GetComponent<Image>().enabled = true;
        joystick_Handler.GetComponent<Image>().enabled = true;

        //Calls the drag function immediately after touching
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystick_Bckgrnd.GetComponent<Image>().enabled = false;
        joystick_Handler.GetComponent<Image>().enabled = false;

        //Resets the position input and joystick handler position
        posInput = Vector2.zero;
        joystick_Handler.rectTransform.anchoredPosition = Vector2.zero;
    }

    public float HorizontalInput()
    {
        float horizontalInput;

        if (posInput.x != 0)
        {
            //Returns the input on x when dragging the screen and clamps it.
            horizontalInput = posInput.x;
            Mathf.Clamp(horizontalInput, -1f, 1f);
        }
        else
        {
            //Optional keyboard input
            horizontalInput = Input.GetAxis("Horizontal");
        }

        return horizontalInput;
    }

    public float ForwardInput()
    {
        float forwardInput;

        if (posInput.y != 0)
        {
            //Returns the input on y when dragging the screen and clamps it.
            forwardInput = posInput.y;
            Mathf.Clamp(forwardInput, -1f, 1f);
        }
        else
        {
            //Optional keyboard input
            forwardInput = Input.GetAxis("Vertical");
        }

        return forwardInput;
    }
    private void GetClickableArea()
    {
        inputAreaSize.x = joystick_Bckgrnd.rectTransform.sizeDelta.x;
        inputAreaSize.y = joystick_Bckgrnd.rectTransform.sizeDelta.y;
    }

}