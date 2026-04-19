using UnityEngine;

public class EnemyView2 : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] private EnemyController2 controller;

    private void Update()
    {
        //anim.SetBool("isMoving", controller.IsMoving);
    }
}
