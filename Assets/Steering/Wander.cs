using UnityEngine;

public class WanderBehaviour : SteeringBehaviour
{
    private Vector3 currentDirection;
    private float changeInterval;
    private float timer;

    public WanderBehaviour(float interval = 2f)
    {
        changeInterval = interval;

        currentDirection = Random.insideUnitSphere;
        currentDirection.y = 0;
        currentDirection.Normalize();
    }

    public override Vector3 GetSteering(Transform agent)
    {
        timer += Time.deltaTime;

        if (timer >= changeInterval)
        {
            timer = 0;

            currentDirection = Random.insideUnitSphere;
            currentDirection.y = 0;
            currentDirection.Normalize();
        }

        return currentDirection;
    }
}