using UnityEngine;

public class Player : MonoBehaviour
{
    public float walkSpeed;
    public float runSpeed;
    public float jumpPower;
    public float dodgeDuration;
    public GameObject[] weapons;
    public bool[] hasWeapons;

    float horizontalAxis;
    float verticalAxis;

    bool runDown;
    bool jumpDown;
    bool interactionDown;
    bool[] swapDowns = new bool[3];

    bool isJump;
    bool isDodge;

    Vector3 moveVector;
    Vector3 dodgeVector;

    Animator animator;
    Rigidbody rigid;

    GameObject nearObject;
    GameObject equipedWeapon;
    int equipedWeaponIndex = -1;

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
        Swap();
        Interaction();
    }

    void KeyInput()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        runDown = Input.GetButton("Run");
        jumpDown = Input.GetButtonDown("Jump");
        interactionDown = Input.GetButtonDown("Interaction");
        swapDowns[0] = Input.GetButtonDown("Swap1");
        swapDowns[1] = Input.GetButtonDown("Swap2");
        swapDowns[2] = Input.GetButtonDown("Swap3");
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

    void Swap()
    {
        int weaponIndex = -1;
        if (swapDowns[0]) weaponIndex = 0;
        else if (swapDowns[1]) weaponIndex = 1;
        else if (swapDowns[2]) weaponIndex = 2;

        if (weaponIndex == -1 || isJump || isDodge)
            return;

        if (!hasWeapons[weaponIndex] || equipedWeaponIndex == weaponIndex)
            return;

        if (equipedWeapon != null)
        {
            equipedWeapon.SetActive(false);
        }

        equipedWeaponIndex = weaponIndex;
        equipedWeapon = weapons[weaponIndex];
        equipedWeapon.SetActive(true);

        animator.SetTrigger("doSwap");
    }

    void Interaction()
    {
        if(interactionDown && nearObject != null && !isJump && !isDodge)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            animator.SetBool("isJump", false);
            isJump = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Weapon")
        {
            nearObject = other.gameObject;
        }

        Debug.Log(nearObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = null;
        }
    }
}
