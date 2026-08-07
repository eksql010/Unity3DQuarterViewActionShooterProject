using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon };
    public Type type;
    public int value;
    public float rotationSpeed;

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);   
    }
}
