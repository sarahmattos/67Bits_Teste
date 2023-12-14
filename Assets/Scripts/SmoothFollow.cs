using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
/*
    Script para aplicar movimentação e rotação suavemente
*/

    [SerializeField] private Transform targetPlayer;// Transform que o objeto deve seguir
    private Vector3 offset;// Diferença entre as posições
    private Vector3 currentVelocity = Vector3.zero;// Velocidade
    private PlayerManager player;// Referência do playerManager
    public bool camera;// Indicador que o objeto é a câmera

    //Valores editaveis dependendo da situação
    [SerializeField] float smoothTime;// Tempo de suavização da movimentação
    [SerializeField] float velocidadeRotacao = 20.0f;// Velocidade da rotação

    void OnEnable()
    {
        player = FindAnyObjectByType<PlayerManager>();
        if (!camera)
        {
            // Quando NPC é pego ele ativa esse script que faz com que obtenha a posição do player 
            //(mantendo sua posição em y que foi passada anteriormente para ficar na altura certa na pilha)
            Vector3 novaPosicao = new Vector3(targetPlayer.position.x, transform.position.y, targetPlayer.position.z);
            transform.position = novaPosicao;
        }
        //Calcula diferença entre a posição atual e a desejada
        offset = transform.position - targetPlayer.position;
    }

    private void LateUpdate()
    {
        // Calcula a nova posição do objecto com base no jogador e na diferença calculada anteriormente e aplica suavidade
        Vector3 targetPosition = targetPlayer.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

        //Não aplica rotação se for a câmera
        if (!camera) Rotation();
    }

    public void ConfigLevel(int id)
    {
        // Configura as propriedades de suavização e velocidade de rotação com base no id do NPC em relação à pilha
        switch (id)
        {
            case 1:
                smoothTime = 0.1f;
                velocidadeRotacao = 10f;
                break;

            case 2:
                smoothTime = 0.25f;
                velocidadeRotacao = 25;
                break;

            case 3:
                smoothTime = 0.4f;
                velocidadeRotacao = 40f;
                break;
            
            default:
                break;
        }
    }

    public void Rotation()
    {
        // Determina as rotações desejadas com base no movimento do jogador
        float rotacaoDesejadaZ = player.scaledMovement.x < 0 ? -velocidadeRotacao : (player.scaledMovement.x > 0 ? velocidadeRotacao : 0f);
        float rotacaoDesejadaX = player.scaledMovement.z > 0 ? -velocidadeRotacao / 2 : (player.scaledMovement.z < 0 ? velocidadeRotacao : 0f);

        // Interpola suavemente as rotações atuais para as rotações desejadas
        float rotacaoSuavizadaZ = Mathf.LerpAngle(transform.eulerAngles.z, rotacaoDesejadaZ, Time.deltaTime * 5.0f);
        float rotacaoSuavizadaX = Mathf.LerpAngle(transform.eulerAngles.x, rotacaoDesejadaX, Time.deltaTime * 5.0f);

        // Aplica as rotações suavizadas ao objeto
        transform.eulerAngles = new Vector3(rotacaoSuavizadaX, transform.eulerAngles.y, rotacaoSuavizadaZ);
    }
}
