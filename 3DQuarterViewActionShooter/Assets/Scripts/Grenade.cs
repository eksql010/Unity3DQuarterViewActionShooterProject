using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObject;
    public GameObject effectObject;
    public Rigidbody rigid;
    public float explosionBeforeDuration;

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
    }
}
