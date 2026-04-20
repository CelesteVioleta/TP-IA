using UnityEngine;

public class FSM : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Investigate
    }

    public EnemyState currentState;

    public void UpdateState(bool canSeePlayer)
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                if (canSeePlayer)
                {
                    currentState = EnemyState.Chase;
                    Debug.Log("Chase");
                }
                break;

            case EnemyState.Chase:
                if (!canSeePlayer)
                {
                    currentState = EnemyState.Patrol;
                    Debug.Log("Patrol");
                }
                break;
        }
    }
}