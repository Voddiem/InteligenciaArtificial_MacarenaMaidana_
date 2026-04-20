using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private LineOfSight los;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private FSMClasses fsm;

    [Header("Variables")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float attackDistance = 2f;

    private int currentPoint = 0;
    void Awake()
    {
        if (los == null)
            los = GetComponent<LineOfSight>();

        if (fsm == null)
            fsm = GetComponent<FSMClasses>();
    }

    void Update()
    {
        bool canSeePlayer =
            los.CheckRange(transform, player)
            && los.CheckAngle(transform, player)
            && los.CheckObstacles(transform, player);

        fsm.UpdateState(canSeePlayer);

       
    }


    public void Patrol()
    {
        //transform.Rotate(Vector3.up * 30f * Time.deltaTime);

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

        transform.position += moveDir * speed * Time.deltaTime;

        transform.forward = Vector3.Lerp(
            transform.forward,
            moveDir,
            Time.deltaTime * rotationSpeed);
    }

    public void PursuitPlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        Vector3 moveDir = dir.normalized;

        transform.position += moveDir * speed * Time.deltaTime;

        transform.forward = Vector3.Lerp(
            transform.forward,
            moveDir,
            Time.deltaTime * rotationSpeed);
    }


}

