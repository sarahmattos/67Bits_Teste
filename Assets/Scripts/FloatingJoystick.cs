using UnityEngine;

public class FloatingJoystick : MonoBehaviour
{
    [HideInInspector]
    public RectTransform RectTransform;// Referência do componente RectTransform
    public RectTransform Knob;// Referência do círculo ao meio do joystick

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();// Encontra o componente rectransform do joystick
    }
}