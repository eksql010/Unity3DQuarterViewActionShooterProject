using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    public float damageKnockbackPower;
    public float deadKnockbackPower;
    public float explosionKnockbackHeight;
    public float explosionRotationPower;

    public Transform targetTransform;
    public bool isChase;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material material;
    NavMeshAgent navAgent;
    Animator animator;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        material = GetComponentInChildren<MeshRenderer>().material;
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2f);
    }

    void Update()
    {
        if (isChase)
        {
            navAgent.SetDestination(targetTransform.position);
        }
    }

    void FixedUpdate()
    {
        FreezeVelocity();
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
        material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            material.color = Color.white;

            reactionVec = reactionVec.normalized;
            reactionVec += Vector3.up;

            rigid.AddForce(reactionVec * damageKnockbackPower, ForceMode.Impulse);
        }
        else
        {
            material.color = Color.gray3;
            gameObject.layer = 11;

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

            Destroy(gameObject, 3f);
        }
    }
}
