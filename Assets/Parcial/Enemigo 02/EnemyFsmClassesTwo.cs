using UnityEngine;
using UnityEngine.XR;

public class FSMClasses : MonoBehaviour
{
    public State currentState { get; private set; }

    private PatrolState patrolState;
    private PursuitState pursuitState;
    private AttackState attackState;
    private SearchState searchState;

    private EnemyController enemy;

    private Transform player;
    private Transform self;
    public Animator animator { get; private set; }


    private void Awake()
    {
        animator = GetComponent<Animator>();

        self = transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;


        enemy = GetComponent<EnemyController>();

        patrolState = new PatrolState(this);
        pursuitState = new PursuitState(this);
        attackState = new AttackState(this, self, player);
        searchState = new SearchState(this, self, player);

        currentState = patrolState;
    }

    public void UpdateState(bool canSeePlayer)
    {
        if (canSeePlayer)
        {
            enemy.LastKnownPlayerPosition = player.position;
        }
        currentState.Update(canSeePlayer);
    }

    public void ChangeState(State newState)
    {
        if (currentState == newState)
            return;


        currentState.Exit();
        currentState = newState;
        currentState.Enter();

    }
    public void ChangeToPatrol()
    {
        ChangeState(patrolState);
    }
    public void ChangeToPursuit()
    {
        ChangeState(pursuitState);
    }
    public void ChangeToAttack()
    {
        ChangeState(attackState);
    }
    public void ChangeToSearch()
    {
        ChangeState(searchState);
    }
}

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
        Debug.Log("Entro a Patrol");

        fsm.animator.SetInteger("State", 0);
    }
    public override void Exit()
    {
        Debug.Log("Salgo Patrol");
    }
    public override void Update(bool canSeePlayer)
    {
        fsm.GetComponent<EnemyController>().Patrol();

        if (canSeePlayer)
        {
            fsm.ChangeToPursuit();
        }
    }
}

public class PursuitState : State
{
    public PursuitState(FSMClasses fsm) : base(fsm) { }
    public override void Enter()
    {
        Debug.Log("Entro a Pursuit");

        fsm.animator.SetInteger("State", 1);

        fsm.GetComponent<EnemyController>().CalculatePath();
    }
    public override void Exit()
    {
        Debug.Log("Salgo Pursuit");
    }
    public override void Update(bool canSeePlayer)
    {
        var enemy = fsm.GetComponent<EnemyController>();

        enemy.PursuitPlayer();

        float dist = Vector3.Distance(enemy.transform.position,
         GameObject.FindGameObjectWithTag("Player").transform.position);

        if (!canSeePlayer)
        {
            Debug.Log("Perdí al jugador");
            fsm.ChangeToSearch();
        }
        else if (dist < enemy.AttackDistance)
        {
            fsm.ChangeToAttack();
        }
    }
}

public class AttackState : State
{
    private Transform player;
    private Transform self;
    private float attackCooldown = 3f;
    private float timer;

    public AttackState(FSMClasses fsm, Transform self, Transform player) : base(fsm)
    {
        this.self = self;
        this.player = player;
    }

    public override void Enter()
    {
        Debug.Log("Atacando");

        fsm.animator.SetInteger("State", 2);

        timer = 0;
    }

    public override void Update(bool canSeePlayer)
    {
        float dist = Vector3.Distance(self.position, player.position);

        if (!canSeePlayer)
        {
            fsm.ChangeToSearch();
            return;
        }

        var enemy = fsm.GetComponent<EnemyController>();

        if (dist > enemy.AttackDistance)
        {
            fsm.ChangeToPursuit();
        }

        timer += Time.deltaTime;

        if (timer >= attackCooldown)
        {
            timer = 0;
            Attack();
        }
    }

    void Attack()
    {
        Debug.Log("Golpe");

        fsm.animator.SetTrigger("Attack");

    }



}
public class SearchState : State
{
    public override void Update(bool canSeePlayer)
    {
        if (canSeePlayer)
        {
            Debug.Log("Lo encontré otra vez");
            fsm.ChangeToPursuit();
            return;
        }
        if (!reachedDestination)
        {
            enemy.PursuitPlayer();

            if (enemy.PathFinished())
            {
                reachedDestination = true;
            }

            return;
        }

        searchTimer += Time.deltaTime;

        enemy.transform.Rotate(
    Vector3.up * 90f * Time.deltaTime);

        if (searchTimer >= 3f)
        {
            fsm.ChangeToPatrol();
        }


    }
    public override void Enter()
    {
        Debug.Log("Buscando jugador");

        searchTimer = 0;

        reachedDestination = false;

        enemy.CalculatePath(enemy.LastKnownPlayerPosition);
    }
    public override void Exit()
    {
        Debug.Log("Salgo Search");
    }

    private EnemyController enemy;

    private float searchTimer;

    private bool reachedDestination;

    public SearchState(FSMClasses fsm, Transform self, Transform player)
        : base(fsm)
    {
        enemy = fsm.GetComponent<EnemyController>();
    }
}
