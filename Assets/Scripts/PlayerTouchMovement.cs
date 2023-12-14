using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerTouchMovement : MonoBehaviour
{
/*
    Script para movimento do jogador via toque, usando um joystick virtual.
    Atualiza a posição do joystick e movimenta o jogador de acordo.
*/

    [SerializeField] private Vector2 JoystickSize = new Vector2(50, 50); // Tamanho do joystick virtual
    [SerializeField] private FloatingJoystick Joystick; //  Referência ao script do Joystick virtual 

    private Finger MovementFinger; // Estado do dedo
    public Vector2 MovementAmount; // Quantidade de movimento do jogador

    // Métodos para tratamento de toque
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleLoseFinger;
        ETouch.Touch.onFingerMove += HandleFingerMove;
    }

    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleLoseFinger;
        ETouch.Touch.onFingerMove -= HandleFingerMove;
        EnhancedTouchSupport.Disable();
    }

    // Movimento do joystick durante o toque
    private void HandleFingerMove(Finger MovedFinger)
    {
        // Verifica se o dedo em movimento é o dedo responsável pelo movimento do joystick
        if (MovedFinger == MovementFinger)
        {
            Vector2 knobPosition;
            float maxMovement = JoystickSize.x / 2f;
            ETouch.Touch currentTouch = MovedFinger.currentTouch;

            // Calcula a posição do joystick com base na posição atual do dedo
            if (Vector2.Distance(currentTouch.screenPosition, Joystick.RectTransform.anchoredPosition) > maxMovement)
            {
                knobPosition = (currentTouch.screenPosition - Joystick.RectTransform.anchoredPosition).normalized * maxMovement;
            }
            else
            {
                knobPosition = currentTouch.screenPosition - Joystick.RectTransform.anchoredPosition;
            }

            // Atualiza a posição visual do joystick 
            Joystick.Knob.anchoredPosition = knobPosition;

            // Calcula a quantidade de movimento para controlar o jogador
            MovementAmount = knobPosition / maxMovement;
        }
    }

    // Lida com a perda do toque
    private void HandleLoseFinger(Finger LostFinger)
    {
        if (LostFinger == MovementFinger)
        {
            MovementFinger = null;
            Joystick.Knob.anchoredPosition = Vector2.zero;
            Joystick.gameObject.SetActive(false);
            MovementAmount = Vector2.zero;
        }
    }

     // Lida com o primeiro toque
    private void HandleFingerDown(Finger TouchedFinger)
    {
        MovementFinger = TouchedFinger;
        MovementAmount = Vector2.zero;
        Joystick.gameObject.SetActive(true);
        Joystick.RectTransform.sizeDelta = JoystickSize;
        Joystick.RectTransform.anchoredPosition = ClampStartPosition(TouchedFinger.screenPosition);
    }

    // Limita a posição do joystick na tela
    private Vector2 ClampStartPosition(Vector2 StartPosition)
    {
        if (StartPosition.x < JoystickSize.x / 2)
        {
            StartPosition.x = JoystickSize.x / 2;
        }

        if (StartPosition.y < JoystickSize.y / 2)
        {
            StartPosition.y = JoystickSize.y / 2;
        }
        else if (StartPosition.y > Screen.height - JoystickSize.y / 2)
        {
            StartPosition.y = Screen.height - JoystickSize.y / 2;
        }

        return StartPosition;
    }
}