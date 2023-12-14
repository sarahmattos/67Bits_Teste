using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDollPhysics : MonoBehaviour
{
    // Start is called before the first frame update
    Collider[] ragDollColliders;
    Rigidbody[] ragDollRigidbodys;
    Animator anim;
    Collider colliderNpc;
    Rigidbody rbNpc;
    public bool rag;
    public float posY;
    void Start()
    {
        ragDollColliders = GetComponentsInChildren<Collider>();
        ragDollRigidbodys = GetComponentsInChildren<Rigidbody>();
        anim = GetComponent<Animator>();
        colliderNpc = GetComponent<Collider>();
        rbNpc = GetComponent<Rigidbody>();
        RagDollDisable();
    }

    // Update is called once per frame
    void Update()
    {
       // if(rag){
           // RagDollEnable();
           // rag=false;
       // }
    }
    public void RagDollEnable(Vector3 force)
    {
        foreach(Collider col in ragDollColliders)
        {
            col.enabled = true;
        }
        foreach(Rigidbody rb in ragDollRigidbodys)
        {
            rb.isKinematic = false;
            rb.AddForce(force, ForceMode.Impulse);
        }

        anim.enabled = false;
        colliderNpc.enabled = false;
        rbNpc.isKinematic = true;
    }
    public void RagDollDisable()
    {
        foreach(Collider col in ragDollColliders)
        {
            col.enabled = false;
        }
        foreach(Rigidbody rb in ragDollRigidbodys)
        {
            rb.isKinematic = true;
        }

        anim.enabled = true;
        colliderNpc.enabled = true;
        rbNpc.isKinematic = false;
        

    }
}
