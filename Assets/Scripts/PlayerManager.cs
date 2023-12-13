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
   public int xp;
   [SerializeField] Renderer render;
   [SerializeField] Material[] materials;
   [SerializeField] TMP_Text countText;
   [SerializeField] TMP_Text  moneyCount;
   [SerializeField] TMP_Text  levelCount;
   [SerializeField] Slider sliderMoney;
   public Vector3 scaledMovement;
   public float lastTargetY;
   Transform lastTarget;
   public List<GameObject> npcsDefeats;
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
         scaledMovement = Player.speed * Time.deltaTime * new Vector3(
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
            
            foreach(GameObject npc in npcsDefeats){
                Destroy(npc);
            }
            if(money>=xp){
                money= money- xp;
                SetLevel();
            }
            moneyCount.text = money.ToString()+"/"+xp;
            sliderMoney.maxValue = xp;
            sliderMoney.value = money;
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        target = other.gameObject;
        if(target.tag =="NPC")
        {
            if(npcCount<capacity){
                Stable();
                Invoke("JoinNPCs", 0.7f);
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
        Destroy(target);
    }

    void ConfigLevel(int level)
    {
        // Use um bloco switch para configurar com base no nível
        switch (level)
        {
            case 1:
                capacity = 1;
                render.material= materials[0];
                xp=20;
                break;

            case 2:
                capacity = 2;
                render.material= materials[1];
                xp=40;
                break;

            case 3:
                capacity = 3;
                render.material= materials[2];
                xp=100;
                break;

            default:
                Debug.LogWarning("Nível desconhecido. Não foi possível configurar.");
                break;
        }
        countText.text = npcCount.ToString()+"/"+capacity;
        moneyCount.text = money.ToString()+"/"+xp;
        sliderMoney.maxValue = xp;
        sliderMoney.value = money;
        levelCount.text = "Level: "+ level.ToString();
        
    }
    public void SetLevel(){
        level++;
        ConfigLevel(level);
        
    }

    private void JoinNPCs()
    {
        npcCount++;
        countText.text = npcCount.ToString()+"/"+capacity;

        if(npcCount==1){
            lastTargetY = this.transform.position.y;
            npcsDefeats = new List<GameObject>();
        }else{
            lastTargetY = lastTarget.position.y;
        }
        
        lastTarget = target.transform;

        target.tag = "Untagged";
        Vector3 novaPosicao = new Vector3(target.transform.position.x, lastTargetY +3f, target.transform.position.z);

            // Atribua a nova posição ao objeto em cima
        target.transform.rotation = Quaternion.identity;
        target.transform.position = novaPosicao;
        target.GetComponent<Animator>().SetTrigger("Defeat");
        target.GetComponent<SmoothCameraFollow>().enabled=true;
        target.GetComponent<SmoothCameraFollow>().ConfigLevel(npcCount);
        npcsDefeats.Add(target);
    }
    
}