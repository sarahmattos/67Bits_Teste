using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
   private Animator anim;// Referência do animator do player
   private PlayerTouchMovement playerTouchMovement;// Referência do script de touch
   private UnityEngine.AI.NavMeshAgent Player;// Referência do componente de navegação do player
   void Start()
   {
        // Encontra os componentes
         anim = GetComponent<Animator>();
         playerTouchMovement = GetComponent<PlayerTouchMovement>();
         Player = GetComponent<UnityEngine.AI.NavMeshAgent>();
   }
   private void Update()
    {
        Move();
    }
    public void Move(){
         // Calcula o movimento baseado na entrada do joystick
        Vector3 scaledMovement = Player.speed * Time.deltaTime * new Vector3(
            playerTouchMovement.MovementAmount.x,
            0,
            playerTouchMovement.MovementAmount.y
        );

         // Faz com que o jogador olhe na direção do movimento
        Player.transform.LookAt(Player.transform.position + scaledMovement, Vector3.up);
        // Move o jogador na direção calculada
        Player.Move(scaledMovement);

        // Atualiza a animação com base no movimento
        anim.SetBool("Moving", scaledMovement != Vector3.zero);
    }
}