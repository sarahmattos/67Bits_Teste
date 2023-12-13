using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerManager : MonoBehaviour
{
   private Animator anim;// Referência do animator do player
   private PlayerTouchMovement playerTouchMovement;// Referência do script de touch
   private UnityEngine.AI.NavMeshAgent Player;// Referência do componente de navegação do player
   GameObject target;
   public int npcCount;
   public int money;
   public int level=1;
   public int capacity;
   [SerializeField] Renderer render;
   [SerializeField] Material[] materials;
   [SerializeField] TMP_Text countText;
   [SerializeField] TMP_Text  moneyCount;
   void Start()
   {
        // Encontra os componentes
         anim = GetComponent<Animator>();
         playerTouchMovement = GetComponent<PlayerTouchMovement>();
         Player = GetComponent<UnityEngine.AI.NavMeshAgent>();
         ConfigLevel(level);
         countText.text = npcCount.ToString()+"/"+capacity;
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
    public void Stable(){
         anim.SetTrigger("Stabling");
    }
    public void GainMoney(){
        if(npcCount>0){
            money += npcCount*10;
            npcCount= 0;
            countText.text = npcCount.ToString()+"/"+capacity;
            moneyCount.text = money.ToString();
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        target = other.gameObject;
        if(target.tag =="NPC")
        {
            if(npcCount<capacity){
                Stable();
                Invoke("DestroyNPC", 0.7f);
            }
            
        }
        if(target.tag =="MoneyArea")
        {
            GainMoney();
            //Invoke("DestroyNPC", 0.7f);
        }
    }
    void DestroyNPC()
    {
        npcCount++;
        countText.text = npcCount.ToString()+"/"+capacity;
        Destroy(target);
    }

    void ConfigLevel(int level)
    {
        // Use um bloco switch para configurar com base no nível
        switch (level)
        {
            case 1:
                capacity = 2;
                render.material= materials[0];
                break;

            case 2:
                capacity = 3;
                render.material= materials[1];
                break;

            case 3:
                capacity = 4;
                render.material= materials[2];
                break;

            default:
                Debug.LogWarning("Nível desconhecido. Não foi possível configurar.");
                break;
        }
    }
    public void SetLevel(){
        level++;
        ConfigLevel(level);
        countText.text = npcCount.ToString()+"/"+capacity;
    }
    
}