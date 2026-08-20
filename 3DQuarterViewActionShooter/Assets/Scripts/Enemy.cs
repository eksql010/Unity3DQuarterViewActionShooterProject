using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    public float damageKnockbackPower;
    public float deadKnockbackPower;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material material;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        material = GetComponent<MeshRenderer>().material;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactionVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactionVec));

            //  Debug.Log("Melee : " + curHealth);
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactionVec = transform.position - other.transform.position;
            
            StartCoroutine(OnDamage(reactionVec));

            //  Debug.Log("Range : " + curHealth);
        }
    }

    IEnumerator OnDamage(Vector3 reactionVec)
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
            reactionVec += Vector3.up;

            rigid.AddForce(reactionVec * deadKnockbackPower, ForceMode.Impulse);

            Destroy(gameObject, 3f);
        }
    }
}
