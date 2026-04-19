using UnityEngine;

public class FSM : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase
    }

    public EnemyState currentState = EnemyState.Patrol;

    public void UpdateState(bool canSeePlayer)
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                if (canSeePlayer)
                {
                    currentState = EnemyState.Chase;
                    Debug.Log("Persiguiendo");
                }
                break;

            case EnemyState.Chase:
                if (!canSeePlayer)
                {
                    currentState = EnemyState.Patrol;
                Debug.Log("Patrullando");
                }
                break;
        }
    }
}
