using UnityEngine;
using UnityEngine.UI;

public class JoystickController : MonoBehaviour
{
    public Image joystickBase;
    public Image joystickHandle;
    private Vector2 inputDirection = Vector2.zero;
    

    private void Start()
    {
        joystickHandle.rectTransform.anchoredPosition = Vector2.zero; // Reset joystick handle
    }

    public void OnPointerDown()
    {
        // Handle input when the player touches the joystick area
        OnDrag();
    }

    public void OnPointerUp()
    {
        // Reset joystick when player releases touch
        inputDirection = Vector2.zero;
        joystickHandle.rectTransform.anchoredPosition = Vector2.zero;
    }

    public void OnDrag()
    {
        // Calculate input direction
        Vector2 joystickPosition = RectTransformUtility.WorldToScreenPoint(
            new Camera(),
            joystickBase.rectTransform.position
        );
        Vector2 input = (Input.mousePosition - (Vector3)joystickPosition) / (joystickBase.rectTransform.sizeDelta.x / 2);

        inputDirection = (input.magnitude > 1.0f) ? input.normalized : input;
        joystickHandle.rectTransform.anchoredPosition = new Vector2(inputDirection.x * (joystickBase.rectTransform.sizeDelta.x / 2),
            inputDirection.y * (joystickBase.rectTransform.sizeDelta.y / 2));
    }

    public Vector2 GetInputDirection()
    {
        return inputDirection;
    }
}
