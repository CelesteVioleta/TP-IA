using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    PlayerModel model;

    private void Awake()
    {
        model = GetComponent<PlayerModel>();
    }

    private void Update()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis ("Vertical");

        Vector3 dir = new Vector3 (hor, 0, ver).normalized;

        model.Walk(dir);
        

        if(hor != 0 || ver != 0 )
        {
            model.Rotate(dir);
        }
            
    }
}
