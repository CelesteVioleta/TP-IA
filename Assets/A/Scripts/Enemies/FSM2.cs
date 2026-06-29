using UnityEngine;

public class FSM2 : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Investigate,
        Attack,
        Evade
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
                    currentState = EnemyState.Attack;
                    Debug.Log("ATTACK");
                }
                else if (hasLastKnownPosition)
                {
                    currentState = EnemyState.Investigate;
                    Debug.Log("INVESTIGATE");
                }
                break;

            case EnemyState.Attack:

                if (isClose)
                {
                    currentState = EnemyState.Evade;
                    Debug.Log("EVADE");
                }
                else if (!canSeePlayer)
                {
                    currentState = EnemyState.Investigate;
                    Debug.Log("INVESTIGATE");
                }

                break;

            case EnemyState.Evade:

                if (!isClose)
                {
                    currentState = EnemyState.Attack;
                    Debug.Log("ATTACK");
                }

                break;

            case EnemyState.Investigate:

                if (canSeePlayer)
                {
                    currentState = EnemyState.Attack;
                    Debug.Log("ATTACK");
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