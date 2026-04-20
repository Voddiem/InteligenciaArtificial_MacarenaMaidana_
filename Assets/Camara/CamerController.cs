using UnityEngine;

public class CamerController : MonoBehaviour
{
    LineOfSight los;
   [SerializeField] GameObject player;
    [SerializeField] GameObject indicator;
    private void Awake()
    {
        los = GetComponent<LineOfSight>();
    }
    // Update is called once per frame
    void Update()
    {
        if (los.CheckRange(transform,player.transform )
            && los.CheckAngle(transform, player.transform)
            && los.CheckObstacles(transform, player.transform))
        {
            //Debug.Log("Lo vio");
            indicator.SetActive(true);
        }
        else
        {
            //Debug.Log("No lo vio");
            indicator.SetActive(false);
        }
    }
}
