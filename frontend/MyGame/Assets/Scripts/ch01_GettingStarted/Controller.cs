using UnityEngine;
using Mirror;

public class Controller : NetworkBehaviour
{
    private float speed = 0.01f;
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (!hasAuthority) return;

        Vector3 movement = new Vector3(Input.GetAxis("Horizontal") * speed, 0, Input.GetAxis("Vertical") * speed);
        transform.position = transform.position + movement;

        if ((movement.x != 0) || (movement.y != 0) || (movement.z != 0))
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(movement), 10000 * Time.deltaTime);
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetBool("moving", false);
        }
    }
}