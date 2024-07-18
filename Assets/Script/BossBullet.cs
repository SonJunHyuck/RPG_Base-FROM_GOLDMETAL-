using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBullet : Bullet
{
    public Transform target;
    private NavMeshAgent navAgent;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        navAgent.SetDestination(target.position);
    }

}
