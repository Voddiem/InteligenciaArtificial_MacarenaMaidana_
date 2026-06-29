using UnityEngine;

public class ArriveBehaviour : SteeringBehaviour
{
    private Vector3 targetPosition;

    private float slowRadius;

    public ArriveBehaviour(float slowRadius = 2f)
    {
        this.slowRadius = slowRadius;
    }

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
    }

    public override Vector3 GetSteering(Transform agent)
    {
        Vector3 dir = targetPosition - agent.position;
        dir.y = 0;

        float distance = dir.magnitude;

        if (distance < 0.05f)
            return Vector3.zero;

        float speedFactor = 1f;

        if (distance < slowRadius)
            speedFactor = distance / slowRadius;

        return dir.normalized * speedFactor;
    }
}