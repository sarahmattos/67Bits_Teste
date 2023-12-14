using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDollPhysics : MonoBehaviour
{
    // Componentes do Ragdoll
    Collider[] ragDollColliders;// Todos colliders que compõe o ragdoll
    Rigidbody[] ragDollRigidbodys;// Todos rigdbodys que compõe o ragdoll
    Animator anim;// Animator do NPC
    Collider colliderNpc;// Collider principal do NPC
    Rigidbody rbNpc;// Rigidbody principal do NPC

    void Start()
    {
        // Inicializa os componentes do Ragdoll
        ragDollColliders = GetComponentsInChildren<Collider>();
        ragDollRigidbodys = GetComponentsInChildren<Rigidbody>();
        anim = GetComponent<Animator>();
        colliderNpc = GetComponent<Collider>();
        rbNpc = GetComponent<Rigidbody>();

        // Desativa o Ragdoll no início
        RagDollDisable();
    }

    // Ativa o Ragdoll e aplica uma força opcional 
    public void RagDollEnable(Vector3 force)
    {
        // Ativa os colliders e rigidbodies do Ragdoll
        foreach (Collider col in ragDollColliders)
        {
            col.enabled = true;
        }

        foreach (Rigidbody rb in ragDollRigidbodys)
        {
            rb.isKinematic = false;
            rb.AddForce(force, ForceMode.Impulse);
        }

        // Desativa o Animator, o Collider principal e o Rigidbody principal
        anim.enabled = false;
        colliderNpc.enabled = false;
        rbNpc.isKinematic = true;

        // Desativa o script SmoothFollow associado
        GetComponent<SmoothFollow>().enabled = false;
    }

    // Desativa o Ragdoll
    public void RagDollDisable()
    {
        // Desativa os colliders e rigidbodies do Ragdoll
        foreach (Collider col in ragDollColliders)
        {
            col.enabled = false;
        }

        foreach (Rigidbody rb in ragDollRigidbodys)
        {
            rb.isKinematic = true;
        }

        // Reativa o Animator, o Collider principal e o Rigidbody principal
        anim.enabled = true;
        colliderNpc.enabled = true;
        rbNpc.isKinematic = false;
    }
}
