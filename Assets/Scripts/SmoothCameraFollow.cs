using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    private Vector3 offset;// Distância da câmera e o player
    [SerializeField] private Transform targetPlayer;// Referência do player
    [SerializeField] private float smoothTime;// Tempo de suavização
    private Vector3 currentVelocity = Vector3.zero;// Velocidade da suavização
    public float velocidadeRotacao = 20.0f;
    float rotacaoSuavizada;
     float rotacaoDesejadaZ,  rotacaoDesejadaX;
    PlayerManager player;
    public bool camera;

    private void Awake(){
        if(camera)offset = transform.position - targetPlayer.position;// Calculo da distância
        player = FindAnyObjectByType<PlayerManager>();
    }
    void OnEnable(){
        
        if(!camera){
            
            Vector3 novaPosicao = new Vector3(targetPlayer.position.x, transform.position.y, targetPlayer.position.z);
            transform.position = novaPosicao;
            offset = transform.position - targetPlayer.position;// Calculo da distância
        }
        
    }
    private void LateUpdate(){
        Vector3 targetPosition = targetPlayer.position + offset;// Calcula a nova posição da câmera
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);// Suaviza o movimento da câmera
        if(!camera)Rotation();
    }
    public void ConfigLevel(int id)
    {
        // Use um bloco switch para configurar com base no nível
        switch (id)
        {
            case 1:
                smoothTime = 0.1f;
                velocidadeRotacao= 10f;
                break;

            case 2:
                smoothTime = 0.25f;
                velocidadeRotacao= 25;
                break;

            case 3:
                smoothTime = 0.4f;
                velocidadeRotacao= 40f;
                break;

            default:
                Debug.LogWarning("Nível desconhecido. Não foi possível configurar.");
                break;
        }
    }
    public void Rotation(){
        

        // Interpola suavemente a rotação atual para a rotação desejada

        // Aplica a rotação suavizada ao objeto
       
     //if(player.scaledMovement.x != 0)
    // {
        //rotacao z
        rotacaoDesejadaZ = player.scaledMovement.x < 0 ? -velocidadeRotacao : (player.scaledMovement.x > 0 ? velocidadeRotacao : 0f);
        rotacaoSuavizada = Mathf.LerpAngle(transform.eulerAngles.z, rotacaoDesejadaZ, Time.deltaTime * 5.0f);
         transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, rotacaoSuavizada);
    // }
    // if(player.scaledMovement.z != 0)
     //{
        //rotacao x
         rotacaoDesejadaX = player.scaledMovement.z > 0 ? -velocidadeRotacao/2 : (player.scaledMovement.z <  0 ? velocidadeRotacao : 0f);
         rotacaoSuavizada = Mathf.LerpAngle(transform.eulerAngles.x, rotacaoDesejadaX, Time.deltaTime * 5.0f);
         transform.eulerAngles = new Vector3(rotacaoSuavizada, transform.eulerAngles.y, transform.eulerAngles.z);
     //}

    }
}
