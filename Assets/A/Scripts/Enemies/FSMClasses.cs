using UnityEngine;

public class FSMClasses : MonoBehaviour
{
    public State currentState {  get; private set; }

    private PatrolState patrolState;
    private ChaseState chaseState;
    private InvestigateState investigateState;
    private AttackState attackState;

    private void Awake()
    {
        patrolState = new PatrolState(this);
        chaseState = new ChaseState(this);
        investigateState = new InvestigateState(this);
        attackState = new AttackState(this);

        currentState = patrolState;
    }

    public void UpdateState(bool canSeePlayer)
    {
        currentState.Update(canSeePlayer);
    }

    public void ChangeState(State newState)
    {
        if (currentState == newState)
        { 
            return;
        }

        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

// Funciones de Cambio de Estados:
    public void ChangeToPatrol()
    {
        ChangeState(patrolState);
    }

    public void ChangeToChase()
    {
        ChangeState(chaseState);
    }

    public void ChangeToInvestigate()
    {
        ChangeState(investigateState);
    }

    public void ChangeToAttackState()
    {
        ChangeState(attackState);
    }
}


// Tipos de Estados:
public abstract class State
{
    protected FSMClasses fsm;

    public State(FSMClasses fsm)
    {
        this.fsm = fsm;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }

    public abstract void Update(bool canSeePlayer);
}

public class PatrolState : State
{
    public PatrolState(FSMClasses fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Entro a Estado Patrol");
    }

    public override void Exit()
    {
        Debug.Log("Salgo de Estado Patrol");
    }

    public override void Update(bool canSeePlayer)
    {
        if (canSeePlayer)
        {
            fsm.ChangeToChase();
        }
    }
}

public class ChaseState : State
{
    public ChaseState(FSMClasses fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Entro a Estado Chase");
    }

    public override void Exit()
    {
        Debug.Log("Salgo de Estado Chase");
    }

    public override void Update(bool canSeePlayer)
    {
        if (!canSeePlayer)
        {
            fsm.ChangeToInvestigate();
        }
    }
}

public class InvestigateState : State
{
    private float timer;
    private float investigateDuration = 10f;

    public InvestigateState(FSMClasses fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Entro a Estado Investigate");
        timer = investigateDuration;
    }

    public override void Exit()
    {
        Debug.Log("Salgo de Estado Investigate");
    }

    public override void Update(bool canSeePlayer)
    {
        if (canSeePlayer)
        {
            fsm.ChangeToChase();
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            fsm.ChangeToPatrol();
        }
    }
}

public class AttackState : State
{
    public AttackState(FSMClasses fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Entro a Estado Attack");
    }

    public override void Exit()
    {
        Debug.Log("Salgo de Estado Attack");
    }

    public override void Update(bool canSeePlayer)
    {
        if (!canSeePlayer)
        {
            fsm.ChangeToChase();
        }
    }
}