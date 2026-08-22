using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, D };
    public Type type;

    public int maxHealth;
    public int curHealth;
    public float damageKnockbackPower;
    public float deadKnockbackPower;
    public float explosionKnockbackHeight;
    public float explosionRotationPower;

    public float attackRadius;
    public float attackRange;
    public float attackBeforeDuration;  // 근접 콜라이더 활성화 전 시간
    public float attackDuration;        // 근접 콜라이더 활성화 후 비활성화 전까지의 시간
    public float attackAfterDuration;   // 공격 종료 후 다음 공격까지의 대기 시간
    //  [SerializeField] float dashForce = 20f;
    public float dashForce;
    public float missileForce;

    public Transform targetTransform;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public bool isChase;
    public bool isAttack;
    public bool isDead;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public NavMeshAgent navAgent;
    public Animator animator;
    public MeshRenderer[] meshs;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();

        if (type != Type.D)
            Invoke("ChaseStart", 2f);
    }

    void Update()
    {
        if (navAgent.enabled && type != Type.D)
        {
            navAgent.SetDestination(targetTransform.position);
            navAgent.isStopped = !isChase;
        }
    }

    void FixedUpdate()
    {
        FreezeVelocity();
        Targeting();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactionVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactionVec, false));

            //  Debug.Log("Melee : " + curHealth);
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactionVec = transform.position - other.transform.position;
            
            StartCoroutine(OnDamage(reactionVec, false));

            //  Debug.Log("Range : " + curHealth);
        }
    }

    public void HitByGrenade(Vector3 explosionPos, int damage)
    {
        curHealth -= damage;
        Vector3 reactionVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactionVec, true));
    }

    IEnumerator OnDamage(Vector3 reactionVec, bool isGrenade)
    {
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.paleVioletRed;
        }

        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.white;
            }

            reactionVec = reactionVec.normalized;
            reactionVec += Vector3.up;

            rigid.AddForce(reactionVec * damageKnockbackPower, ForceMode.Impulse);
        }
        else
        {
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.gray;
            }

            gameObject.layer = 11;

            isDead = true;
            isChase = false;
            navAgent.enabled = false;
            animator.SetTrigger("doDie");

            reactionVec = reactionVec.normalized;
            reactionVec += (isGrenade ? Vector3.up * explosionKnockbackHeight : Vector3.up);

            rigid.AddForce(reactionVec * deadKnockbackPower, ForceMode.Impulse);

            if (isGrenade)
            {
                rigid.freezeRotation = false;
                Vector3 torqueAxis = Random.insideUnitSphere;
                rigid.AddTorque(torqueAxis * explosionRotationPower, ForceMode.Impulse);
            }

            if (type != Type.D)
                Destroy(gameObject, 3f);
        }
    }

    void ChaseStart()
    {
        isChase = true;
        animator.SetBool("isWalk", true);
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.linearVelocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void Targeting()
    {
        if (type == Type.D || isDead)
            return;

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
                                                     attackRadius,
                                                     transform.forward,
                                                     attackRange,
                                                     LayerMask.GetMask("Player"));

        if (rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        animator.SetBool("isAttack", true);

        switch (type)
        {
            case Type.A:
                yield return new WaitForSeconds(attackBeforeDuration);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(attackDuration);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(attackAfterDuration);
                break;

            case Type.B:
                yield return new WaitForSeconds(attackBeforeDuration);
                rigid.AddForce(transform.forward * dashForce, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(attackDuration);
                rigid.linearVelocity = Vector3.zero;
                meleeArea.enabled = false;
                
                yield return new WaitForSeconds(attackAfterDuration);
                break;

            case Type.C:
                yield return new WaitForSeconds(attackBeforeDuration);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.linearVelocity = transform.forward * missileForce;

                yield return new WaitForSeconds(attackAfterDuration);
                break;
        }

        isChase = true;
        isAttack = false;
        animator.SetBool("isAttack", false);
    }
}
