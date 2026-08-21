using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    public float damageKnockbackPower;
    public float deadKnockbackPower;
    public float explosionKnockbackHeight;
    public float explosionRotationPower;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material material;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        material = GetComponentInChildren<MeshRenderer>().material;
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

            reactionVec = reactionVec.normalized;
            reactionVec += (isGrenade ? Vector3.up * explosionKnockbackHeight : Vector3.up);

            rigid.AddForce(reactionVec * deadKnockbackPower, ForceMode.Impulse);

            if (isGrenade)
            {
                rigid.freezeRotation = false;
                rigid.AddTorque(reactionVec * explosionRotationPower, ForceMode.Impulse);
            }

            Destroy(gameObject, 3f);
        }
    }
}
