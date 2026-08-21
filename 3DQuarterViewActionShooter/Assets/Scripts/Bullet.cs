using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isMelee && (other.gameObject.tag == "Wall" || other.gameObject.tag == "Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
