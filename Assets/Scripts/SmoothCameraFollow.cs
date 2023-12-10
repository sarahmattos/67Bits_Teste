using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    private Vector3 offset;// Distância da câmera e o player
    [SerializeField] private Transform targetPlayer;// Referência do player
    [SerializeField] private float smoothTime;// Tempo de suavização
    private Vector3 currentVelocity = Vector3.zero;// Velocidade da suavização

    private void Awake(){
        offset = transform.position - targetPlayer.position;// Calculo da distância
    }
    private void LateUpdate(){
        Vector3 targetPosition = targetPlayer.position + offset;// Calcula a nova posição da câmera
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);// Suaviza o movimento da câmera

    }
}
