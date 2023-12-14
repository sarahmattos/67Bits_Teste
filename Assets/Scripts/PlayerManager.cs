using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    // Referências
    Animator anim;// Referência do animator do player
    PlayerTouchMovement playerTouchMovement;// Referência do script de touch
    UnityEngine.AI.NavMeshAgent Player;// Referência do componente de navegação do player
    GameObject target;// Objeto detectado pelo trigger
    GameObject npc;// Objeto detectado pelo trigger e com tag NPC

    // Variáveis de jogo
    int npcCount;// Contagem de NPCs na mochila
    int capacity;// Capacidade total de NPCs na mochila
    int level = 1;// Level atual do jogo
    int money;// Dinheiro atual do player
    int moneyNeeded;// Dinheiro necessário para passar de nível
    float lastTargetY;// Valor em Y para pilha de NPCs
    Transform lastTarget;// Transform do ultimo NPC na pilha
    [HideInInspector] public Vector3 scaledMovement;// Indicador da movimentação
    bool victory;// Condição para vitoria
    List<GameObject> npcsDefeats;// Lista dos NPCs em pilha

    // UI
    [SerializeField] TMP_Text countText;// Indicador de NPCs na mochila
    [SerializeField] TMP_Text moneyCount;// Indicador de dinheiro atual no ponto de entrega
    [SerializeField] TMP_Text moneyCountUI;// Indicador de dinheiro atual na tela
    [SerializeField] TMP_Text levelCount;// Indicador de level no ponto de entrega
    [SerializeField] TMP_Text levelCountUI;// Indicador de level na tela
    [SerializeField] Slider sliderMoney;// Slider de progresso do level
    [SerializeField] Image backgrounSlider;// Imagem do slider que muda de cor com o level

    // Materiais
    [SerializeField] Renderer render;// Render do player para mudar de material com o level
    [SerializeField] Material[] materials;// Referência dos materias

    void Start()
    {
        // Encontra os componentes
        anim = GetComponent<Animator>();
        playerTouchMovement = GetComponent<PlayerTouchMovement>();
        Player = GetComponent<UnityEngine.AI.NavMeshAgent>();
        //Inicia no level 1
        ConfigLevel(level);
    }

    private void Update()
    {
        if(!victory)Move();
    }

    // Método de movimentação
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

        // Atualiza a animação (run/idle)
        anim.SetBool("Moving", scaledMovement != Vector3.zero);
    }

    // Método chamado quando o jogador colide trigerr com um objeto
    private void OnTriggerEnter(Collider other)
    {
        target = other.gameObject;

        if (target.tag == "NPC")
        {
            // Verifica se o jogador pode aceitar mais NPCs
            if (npcCount < capacity)
            {
                npc = target;
                npc.tag = "Untagged";
                anim.SetTrigger("Stabling");
                Invoke("Stable", 0.7f);
            }
        }

        // Verifica se o objeto é uma área de dinheiro
        if (target.tag == "MoneyArea")
        {
            // Verifica se o jogador tem NPCs para lançar
            if (npcCount > 0)
            {
                anim.SetTrigger("Throw");
                Invoke("Throw", 0.5f);
            }
        }
    }

    // Método chamado quando o jogador chega perto de NPCs e bate
    public void Stable()
    {
        // Aplica uma força e ativa a física de Ragdoll no NPC
        Vector3 forceToAdd = transform.forward * 20f + transform.up * 20;
        npc.GetComponent<RagDollPhysics>().RagDollEnable(forceToAdd);

        Invoke("JoinNPCs", 2f);
    }

    // Une NPCs derrotados ao jogador
    private void JoinNPCs()
    {
        // Desativa o Ragdoll nos NPCs
        npc.GetComponent<RagDollPhysics>().RagDollDisable();

        npcCount++;
        countText.text = npcCount.ToString() + "/" + capacity;

        // Se for o primeiro NPC, usa a posição em Y do player como referência
        if (npcCount == 1)
        {
            lastTargetY = this.transform.position.y;
            npcsDefeats = new List<GameObject>();
        }
        else
        {
        //se não for o primeiro, usa a referência de altura do último NPC
            lastTargetY = lastTarget.position.y;
        }

        // Salva o último NPC derrotado para futuras referências
        lastTarget = npc.transform;

        // Calcula a nova posição para o NPC derrotado
        Vector3 novaPosicao = new Vector3(npc.transform.position.x, lastTargetY + 2f, npc.transform.position.z);
        npc.transform.rotation = Quaternion.identity;
        npc.transform.position = novaPosicao;

        // Ativa a animação de derrota no NPC
        npc.GetComponent<Animator>().SetTrigger("Defeat");

        // Ativa o script para seguir o player suavemente e configura seus valores
        npc.GetComponent<SmoothFollow>().enabled = true;
        npc.GetComponent<SmoothFollow>().ConfigLevel(npcCount);

        // Adiciona o NPC à lista de NPCs derrotados
        npcsDefeats.Add(npc);
    }

    // Método chamado para lançar NPCs
    void Throw()
    {
        foreach (GameObject npc in npcsDefeats)
        {
            // Aplica uma força e ativar física de Ragdoll no NPC durante o lançamento
            Vector3 forceToAdd = transform.forward * 20f + transform.up * 20f;
            npc.GetComponent<RagDollPhysics>().RagDollEnable(forceToAdd);
        }

        Invoke("GainMoney", 1f);
    }

    // Método chamado quando o jogador ganha dinheiro
    public void GainMoney()
    {
        if (npcCount > 0)
        {
            money += npcCount * 10;
            npcCount = 0;
            countText.text = npcCount.ToString() + "/" + capacity;

            // Destroi os NPCs derrotados
            foreach (GameObject npc in npcsDefeats)
            {
                Destroy(npc);
            }

            if (money >= moneyNeeded)
            {
                // Avança para o próximo nível
                money = money - moneyNeeded;
                level++;
                ConfigLevel(level);
            }

            UpdateInfos();
        }
    }

    // Configura os parâmetros do nível atual
    void ConfigLevel(int level)
    {
        switch (level)
        {
            case 1:
                capacity = 1;
                render.material = materials[0];
                backgrounSlider.color = Color.white;
                moneyNeeded = 20;
                break;

            case 2:
                capacity = 2;
                render.material = materials[1];
                backgrounSlider.color = Color.yellow;
                moneyNeeded = 40;
                break;

            case 3:
                capacity = 3;
                render.material = materials[2];
                backgrounSlider.color = new Color(0.6784f, 0.8471f, 0.902f, 1f);
                moneyNeeded = 100;
                break;

            // Configurações após todos os níveis serem concluídos
            default:
                victory = true;
                anim.SetTrigger("Tweark");
                break;
        }
        UpdateInfos();
    }

    // Atualiza os textos e sliders relacionados ao nível, dinheiro e npcs
    public void UpdateInfos()
    {
        countText.text = npcCount.ToString() + "/" + capacity;
        moneyCount.text = money.ToString() + "/" + moneyNeeded;
        moneyCountUI.text = money.ToString() + "/" + moneyNeeded;
        sliderMoney.maxValue = moneyNeeded;
        sliderMoney.value = money;
        levelCount.text = "Level: " + level.ToString();
        levelCountUI.text = "Level: " + level.ToString();
    }

    
}

