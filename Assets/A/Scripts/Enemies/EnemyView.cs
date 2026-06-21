using UnityEngine;

public class EnemyView : MonoBehaviour
{
    Animator anim;
    Rigidbody rb;
    EnemyController2 controller;

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
