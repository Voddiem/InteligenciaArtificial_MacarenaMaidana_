using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float hor, ver;
    PlayerModel model;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        model = GetComponent<PlayerModel>();
    }


    void Update()
    {
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");
        
        animator.SetFloat("VelX", hor);
        animator.SetFloat("VelY", ver);

    }

    void FixedUpdate()
    {
        Vector3 dir = new Vector3(hor, 0, ver);

        model.Walk(dir);

        if (hor != 0 || ver != 0)
        {
            model.Rotate(dir);
        }
    }
}
