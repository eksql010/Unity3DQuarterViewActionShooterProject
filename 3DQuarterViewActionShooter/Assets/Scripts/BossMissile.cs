using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet
{
    public Transform targetTransform;
    NavMeshAgent navAgent;

    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        navAgent.SetDestination(targetTransform.position);
    }
}
