using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Vector2 currentDir; // Direção atual calculada com SmoothDamp
    Vector2 currentDirVelocity; // Velocidade da direção atual para SmoothDamp
    Vector2 lastMoveDirection; // Usado no input do joystick para movimentação
    CharacterController characterController; // Referência ao CharacterController do jogador

    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f; // Tempo suavizado para a direção de movimento
    [SerializeField] float speed = 6.0f; // Velocidade de movimento do jogador

    void Start()
    {
        characterController = GetComponent<CharacterController>(); // Obtém o CharacterController no início
    }

    void Update()
    {
        UpdateMoveFPS(lastMoveDirection); // Atualiza o movimento do jogador com base na última direção de movimento
    }

    void UpdateMoveFPS(Vector2 InputMove)
    {
        InputMove.Normalize(); // Normaliza a direção de entrada para garantir que tenha comprimento 1
        currentDir = Vector2.SmoothDamp(currentDir, InputMove, ref currentDirVelocity, moveSmoothTime); // Suaviza a direção de movimento
        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * speed; // Calcula a velocidade total do jogador
        if (InputMove.magnitude > 0.1f) characterController.Move(velocity * Time.deltaTime); // Move o jogador usando o CharacterController
        
    }

    public void VirtualMoveInput(Vector2 virtualMoveDirection)
    {
        lastMoveDirection = virtualMoveDirection; // Atualiza a direção de movimento com base no input virtual 
    }
}