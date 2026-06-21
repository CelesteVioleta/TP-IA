using UnityEngine;

public class FSM : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Investigate,
        Attack
    }

    public EnemyState currentState = EnemyState.Patrol;

    public void UpdateState(
        bool canSeePlayer,
        bool isClose,
        bool hasLastKnownPosition,
        bool investigationFinished)
    {
        switch (currentState)
        {
            case EnemyState.Patrol:

                if (canSeePlayer)
                {
                    currentState = EnemyState.Chase;
                    Debug.Log("CHASE");
                }

                break;

            case EnemyState.Chase:

                if (isClose)
                {
                    currentState = EnemyState.Attack;
                    Debug.Log("ATTACK");
                }
                else if (!canSeePlayer)
                {
                    currentState = EnemyState.Investigate;
                    Debug.Log("INVESTIGATE");
                }

                break;

            case EnemyState.Attack:

                if (!isClose)
                {
                    currentState = EnemyState.Chase;
                    Debug.Log("CHASE");
                }

                break;

            case EnemyState.Investigate:

                if (canSeePlayer)
                {
                    currentState = EnemyState.Chase;
                    Debug.Log("CHASE");
                }
                else if (investigationFinished || !hasLastKnownPosition)
                {
                    currentState = EnemyState.Patrol;
                    Debug.Log("PATROL");
                }

                break;
        }
    }
}