using UnityEngine;


public class DecisionTreeOne : MonoBehaviour
{
    private EnemyOneController enemy;
    private LineOfSightOne los;
    private Transform player;

    private bool searchStarted;
    private bool reachedDestination;
    private float searchTimer;

    private void Awake()
    {
        enemy = GetComponent<EnemyOneController>();
        los = GetComponent<LineOfSightOne>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame

    void Update()
    {
        bool canSeePlayer = los.CheckRange(transform, player) && los.CheckAngle(transform, player) && los.CheckObstacles(transform, player);

        if (canSeePlayer)
        {
            enemy.LastKnownPlayerPosition = player.position;
            searchStarted = false;
            reachedDestination = false;
            searchTimer = 0;
        }

        float dist =
            Vector3.Distance(
                transform.position,
                player.position);

        if (canSeePlayer)
        {
            AttackOrPursuit(dist);
        }
        else
        {
            SearchOrPatrol();
        }
    }
    private void AttackOrPursuit(float dist)
    {
        if (dist < enemy.AttackDistance)
        {
            Attack();
        }
        else
        {
            Pursuit();
        }
    }
    private void SearchOrPatrol()
    {
        if (enemy.PathFinished())
        {
            Patrol();
        }
        else
        {
            Search();
        }
    }
    private void Patrol()
    {
        enemy.Patrol();
    }

    private void Pursuit()
    {
        enemy.CalculatePath();
        enemy.PursuitPlayer();
    }

    private void Search()
    {
        if (!searchStarted)
        {
            enemy.CalculatePath(enemy.LastKnownPlayerPosition);

            searchStarted = true;
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
            searchStarted = false;
            reachedDestination = false;
            searchTimer = 0;

            Patrol();
        }
    }

    private void Attack()
    {
        Debug.Log("Atacar");
        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        transform.forward = Vector3.Lerp(
            transform.forward,
            dir.normalized,
            5f * Time.deltaTime);
    }
}
