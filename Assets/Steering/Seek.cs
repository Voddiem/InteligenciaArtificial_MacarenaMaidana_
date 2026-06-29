using UnityEngine;

public class SeekBehaviour : SteeringBehaviour
{
    private Vector3 targetPosition;

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
    }

    public override Vector3 GetSteering(Transform agent)
    {
        Vector3 dir = targetPosition - agent.position;
        dir.y = 0;

        return dir.normalized;
    }
}