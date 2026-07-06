using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{
    public enum PatrolType
    {
        Waypoints,
        Wander
    }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private LineOfSight los;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private FSMClasses fsm;

    [Header("Variables")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float attackDistance = 1f;
    [SerializeField] private PatrolType patrolType;
    public float AttackDistance => attackDistance;


    private SeekBehaviour seek;
    private WanderBehaviour wander;
    private ArriveBehaviour arrive;
    private AStar pathfinder;


    private int currentPoint = 0;
    void Awake()
    {
        if (los == null)
            los = GetComponent<LineOfSight>();

        if (fsm == null)
            fsm = GetComponent<FSMClasses>();

        seek = new SeekBehaviour();
        wander = new WanderBehaviour();
        arrive = new ArriveBehaviour(2.5f);

        pathfinder = FindFirstObjectByType<AStar>();


    }

    private List<Node> currentPath = new List<Node>();
    private Vector3 lastKnownPlayerPosition;
    public Vector3 LastKnownPlayerPosition
    {
        get => lastKnownPlayerPosition;
        set => lastKnownPlayerPosition = value;
    }

    private int currentNodeIndex;

    void Update()
    {
        bool canSeePlayer =
            los.CheckRange(transform, player)
            && los.CheckAngle(transform, player)
            && los.CheckObstacles(transform, player);

        fsm.UpdateState(canSeePlayer);

    }

    private void Move(Vector3 direction)
    {
        if (direction == Vector3.zero)
            return;

        transform.position += direction * speed * Time.deltaTime;

        transform.forward = Vector3.Lerp(
            transform.forward,
            direction,
            rotationSpeed * Time.deltaTime);
    }


    public void Patrol()
    {
        if (patrolType == PatrolType.Waypoints)
            PatrolWaypoints();
        else
            PatrolWander();
    }


    private void PatrolWaypoints()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        Transform target = patrolPoints[currentPoint];

        Vector3 dir = target.position - transform.position;
        dir.y = 0;

        if (dir.magnitude < 0.2f)
        {
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
            return;
        }

        Vector3 moveDir = dir.normalized;

        Move(moveDir);
    }

    private void PatrolWander()
    {
        Vector3 moveDir = wander.GetSteering(transform);

        Move(moveDir);
    }

    public void PursuitPlayer()
    {
        FollowPath();
    }
    public void CalculatePath()
    {
        currentPath = pathfinder.FindPath(
            transform.position,
            player.position);

        currentNodeIndex = 0;
    }
    public void CalculatePath(Vector3 targetPosition) //Para el search
    {
        currentPath = pathfinder.FindPath(
            transform.position,
            targetPosition);

        currentNodeIndex = 0;
    }

    private void FollowPath()
    {
        if (currentPath == null || currentPath.Count == 0)
            return;

        Node targetNode = currentPath[currentNodeIndex];

        if (currentNodeIndex == currentPath.Count - 1)
        {
            arrive.SetTarget(targetNode.worldPosition);

            Move(arrive.GetSteering(transform));
        }
        else
        {
            seek.SetTarget(targetNode.worldPosition);

            Move(seek.GetSteering(transform));
        }

        if (Vector3.Distance(transform.position, targetNode.worldPosition) < 0.3f)
        {
            currentNodeIndex++;

            if (currentNodeIndex >= currentPath.Count)
                currentNodeIndex = currentPath.Count - 1;
        }
    }

    public bool PathFinished()
    {
        if (currentPath == null || currentPath.Count == 0)
            return true;

        return currentNodeIndex >= currentPath.Count - 1 &&
               Vector3.Distance(
                   transform.position,
                   currentPath[currentPath.Count - 1].worldPosition) < 0.2f;
    }

    private void OnDrawGizmos()
    {
        if (currentPath == null)
            return;

        Gizmos.color = Color.green;

        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Gizmos.DrawLine(
                currentPath[i].worldPosition,
                currentPath[i + 1].worldPosition);
        }
    }
}
