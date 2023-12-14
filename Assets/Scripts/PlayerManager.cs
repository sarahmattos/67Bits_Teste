using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    // Referências
    private Animator anim;  // Referência do animator do player
    private PlayerTouchMovement playerTouchMovement;  // Referência do script de touch
    private UnityEngine.AI.NavMeshAgent Player;  // Referência do componente de navegação do player
    GameObject target;
    Rigidbody rb;  // Rigidbody temporário
    GameObject npc;

    // Variáveis de jogo
    public int npcCount;
    public int money;
    public int level = 1;
    public int capacity;
    public int xp;
    public float lastTargetY;
    public Vector3 scaledMovement;
    bool victory;
    Transform lastTarget;
    public List<GameObject> npcsDefeats;

    // UI
    [SerializeField] TMP_Text countText;
    [SerializeField] TMP_Text moneyCount;
    [SerializeField] TMP_Text moneyCountUI;
    [SerializeField] TMP_Text levelCount;
    [SerializeField] TMP_Text levelCountUI;
    [SerializeField] Slider sliderMoney;
    [SerializeField] Image backgrounSlider;

    // Materiais
    [SerializeField] Renderer render;
    [SerializeField] Material[] materials;

    void Start()
    {
        // Encontra os componentes
        anim = GetComponent<Animator>();
        playerTouchMovement = GetComponent<PlayerTouchMovement>();
        Player = GetComponent<UnityEngine.AI.NavMeshAgent>();
        ConfigLevel(level);
        countText.text = npcCount.ToString() + "/" + capacity;
    }

    private void Update()
    {
        if(!victory)Move();
    }

    public void Move()
    {
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

    public void Stable()
    {
        rb = npc.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 forceToAdd = transform.forward * 20f + transform.up * 20;
            npc.GetComponent<RagDollPhysics>().RagDollEnable(forceToAdd);
        }

        Invoke("JoinNPCs", 2f);
    }

    public void GainMoney()
    {
        if (npcCount > 0)
        {
            money += npcCount * 10;
            npcCount = 0;
            countText.text = npcCount.ToString() + "/" + capacity;
            sliderMoney.maxValue = xp;
            sliderMoney.value = money;
        
            foreach (GameObject npc in npcsDefeats)
            {
                Destroy(npc);
            }

            if (money >= xp)
            {
                money = money - xp;
                SetLevel();
            }

            moneyCount.text = money.ToString() + "/" + xp;
            moneyCountUI.text = money.ToString() + "/" + xp;
            sliderMoney.maxValue = xp;
            sliderMoney.value = money;
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        target = other.gameObject;

        if (target.tag == "NPC")
        {
            if (npcCount < capacity)
            {
                npc = target;
                npc.tag = "Untagged";
                anim.SetTrigger("Stabling");
                Invoke("Stable", 0.7f);
            }
        }

        if (target.tag == "MoneyArea")
        {
            if (npcCount > 0)
            {
                anim.SetTrigger("Throw");
                Invoke("Throw", 0.5f);
            }
        }
    }

    void Throw()
    {
        foreach (GameObject npc in npcsDefeats)
        { 
            Vector3 forceToAdd = transform.forward * 20f + transform.up * 20f;
            npc.GetComponent<RagDollPhysics>().RagDollEnable(forceToAdd);
            
        }

        Invoke("GainMoney", 1f);
    }

    void ConfigLevel(int level)
    {
        // Use um bloco switch para configurar com base no nível
        switch (level)
        {
            case 1:
                capacity = 1;
                render.material = materials[0];
                backgrounSlider.color =  Color.white;
                xp = 20;
                break;

            case 2:
                capacity = 2;
                render.material = materials[1];
                backgrounSlider.color = Color.yellow;
                xp = 40;
                break;

            case 3:
                capacity = 3;
                render.material = materials[2];
                backgrounSlider.color =new Color(0.6784f, 0.8471f, 0.902f, 1f);
                xp = 100;
                break;

            default:
                victory=true;
                anim.SetTrigger("Tweark");
                break;
        }

        countText.text = npcCount.ToString() + "/" + capacity;
        moneyCount.text = money.ToString() + "/" + xp;
        moneyCountUI.text = money.ToString() + "/" + xp;
        sliderMoney.maxValue = xp;
        sliderMoney.value = money;
        levelCount.text = "Level: " + level.ToString();
        levelCountUI.text = "Level: " + level.ToString();
    }

    public void SetLevel()
    {
        level++;
        ConfigLevel(level);
    }

    private void JoinNPCs()
    {
        npc.GetComponent<RagDollPhysics>().RagDollDisable();
        npcCount++;
        countText.text = npcCount.ToString() + "/" + capacity;

        if (npcCount == 1)
        {
            lastTargetY = this.transform.position.y;
            npcsDefeats = new List<GameObject>();
        }
        else
        {
            lastTargetY = lastTarget.position.y;
        }

        lastTarget = target.transform;

        Vector3 novaPosicao = new Vector3(target.transform.position.x, lastTargetY + 2f, target.transform.position.z);

        npc.transform.rotation = Quaternion.identity;
        npc.transform.position = novaPosicao;
        npc.GetComponent<Animator>().SetTrigger("Defeat");
        npc.GetComponent<SmoothCameraFollow>().enabled = true;
        npc.GetComponent<SmoothCameraFollow>().ConfigLevel(npcCount);
        npcsDefeats.Add(npc);
}
}

