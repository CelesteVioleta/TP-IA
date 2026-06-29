using UnityEngine;

public class Enemy2View : MonoBehaviour
{
    Animator anim;
    Rigidbody rb;
    Enemy2ControllerFSM controller;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        anim.SetFloat("Vel", rb.linearVelocity.magnitude);
    }

    public void PlayAttack()
    {
        anim.SetTrigger("Attack");
    }
}
