using UnityEngine;

public class Player : MonoBehaviour
{
    public float walkSpeed;
    public float runSpeed;
    public float jumpPower;
    public float dodgeDuration;

    float horizontalAxis;
    float verticalAxis;
    bool runDown;
    bool jumpDown;

    bool isJump;
    bool isDodge;

    Vector3 moveVector;
    Vector3 dodgeVector;

    Animator animator;
    Rigidbody rigid;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rigid = GetComponentInChildren<Rigidbody>();
    }

    void Update()
    {
        KeyInput();
        Move();
        Turn();
        Jump();
        Dodge();
    }

    void KeyInput()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        runDown = Input.GetButton("Run");
        jumpDown = Input.GetButtonDown("Jump");
    }

    void Move()
    {
        moveVector = new Vector3(horizontalAxis, 0, verticalAxis).normalized;

        if(isDodge)
        {
            moveVector = dodgeVector;
        }

        transform.position += moveVector * (runDown ? runSpeed : walkSpeed) * Time.deltaTime;

        animator.SetBool("isWalk", moveVector != Vector3.zero);
        animator.SetBool("isRun", runDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVector);
    }

    void Jump()
    {
        if (jumpDown && moveVector == Vector3.zero && !isJump && !isDodge)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            animator.SetBool("isJump", true);
            animator.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Dodge()
    {
        if(jumpDown && moveVector != Vector3.zero && !isJump && !isDodge)
        {
            dodgeVector = moveVector;

            walkSpeed *= 2f;
            runSpeed *= 2f;
            animator.SetTrigger("doDodge");
            isDodge = true;

            Invoke("ExitDodge", dodgeDuration);
        }
    }

    void ExitDodge()
    {
        walkSpeed *= 0.5f;
        runSpeed *= 0.5f;
        isDodge = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            animator.SetBool("isJump", false);
            isJump = false;
        }
    }
}
