using UnityEditor.UI;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField] private int distance;
    [SerializeField] private int angle;
    [SerializeField] private LayerMask obs;
    public bool CheckRange(Transform self, Transform target)
    {
        return Vector3.Distance(self.position, target.position) < distance;

        //if(Vector3.Distance(self.position, target.position) < distance)
        //{
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
    }

    public bool CheckAngle(Transform self, Transform target)
    {
        Vector3 dir = target.position - self.position;
        return Vector3.Angle(self.forward, dir) < angle / 2;
    }

    public bool CheckObstacles(Transform self, Transform target)
    {
        Vector3 dir = target.position - self.position;
        return !Physics.Raycast(self.position, dir.normalized, dir.magnitude, obs);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distance);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * distance);
    }
}
