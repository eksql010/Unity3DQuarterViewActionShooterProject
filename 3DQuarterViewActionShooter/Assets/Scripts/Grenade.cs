using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObject;
    public GameObject effectObject;
    public Rigidbody rigid;
    public float explosionBeforeDuration;
    public float explosionRadius;
    public float explosionAfterLifetime;
    public int damage;

    void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(explosionBeforeDuration);

        rigid.linearVelocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

        meshObject.SetActive(false);
        effectObject.SetActive(true);

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 
                                                     explosionRadius, 
                                                     Vector3.up, 
                                                     0f, 
                                                     LayerMask.GetMask("Enemy"));

        foreach (RaycastHit hitObject in rayHits)
        {
            hitObject.transform.GetComponent<Enemy>().HitByGrenade(transform.position, damage);
        }

        Destroy(gameObject, explosionAfterLifetime);
    }
}
