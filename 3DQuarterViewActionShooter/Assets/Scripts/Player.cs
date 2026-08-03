using UnityEngine;

public class Player : MonoBehaviour
{
    public float walkSpeed;
    public float runSpeed;

    float horizontalAxis;
    float verticalAxis;
    bool runDown;

    Vector3 moveVector;

    Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        runDown = Input.GetButton("Run");

        moveVector = new Vector3(horizontalAxis, 0, verticalAxis).normalized;

        transform.position += moveVector * (runDown ? runSpeed : walkSpeed) * Time.deltaTime;

        animator.SetBool("isWalk", moveVector != Vector3.zero);
        animator.SetBool("isRun", runDown);

        transform.LookAt(transform.position + moveVector);
    }
}
