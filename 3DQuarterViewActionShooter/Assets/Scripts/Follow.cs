using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform targetTransform;
    public Vector3 offset;

    void Update()
    {
        transform.position = targetTransform.position + offset;
    }
}
