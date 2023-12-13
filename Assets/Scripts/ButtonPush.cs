using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ButtonPush : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    PlayerManager player;
    void Start()
    {
        player = FindAnyObjectByType<PlayerManager>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //isPressed = true;
        player.Stable();
        Debug.Log("Botão pressionado!");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //isPressed = false;
        Debug.Log("Botão liberado!");
    }

}
