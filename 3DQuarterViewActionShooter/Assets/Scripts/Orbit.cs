using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform targetTransform;
    public float orbitSpeed;
    Vector3 offset;

    void Start()
    {
        offset = transform.position - targetTransform.position;    
    }

    void Update()
    {
        transform.position = targetTransform.position + offset;
        transform.RotateAround(targetTransform.position, 
                               Vector3.up, 
                               orbitSpeed * Time.deltaTime);
        offset = transform.position - targetTransform.position;
    }
}
