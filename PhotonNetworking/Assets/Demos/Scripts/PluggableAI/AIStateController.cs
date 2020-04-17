﻿using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIStateController : MonoBehaviour
{
    [SerializeField]
    private AIState m_CurrentState;

    [SerializeField]
    private Transform m_Eye;

    public NavMeshAgent Agent { get; private set; }
    public Transform Eye => m_Eye;

    public float TimeSinceAwakening { get; private set; } = 0;
    public float SpawnHeight { get; private set; }

    private AIStateData m_Data;

    private void Awake()
    {
        SpawnHeight = transform.position.y;
        m_Data = new AIStateData(this);
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        TimeSinceAwakening += Time.deltaTime;
        m_CurrentState.UpdateState(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Patrollable")
        {
            m_Data.UpdatePatrollables(new Patrollable(other.transform, other.gameObject.GetInstanceID()));
        }
    }

    public AIDataContainer GetData(AIStateDataType _type)
    {
        return m_Data.GetData(_type);
    }

    public bool TryTransition(AIState nextState)
    {
        if (nextState != m_CurrentState)
        {
            m_CurrentState = nextState;
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        var forward = Eye.forward;
        var position = m_Eye.transform.position + forward;
        var radius = 0.25f;
        for (int i = 0; i < 5; i++)
        {
            Gizmos.DrawSphere(position + (forward * i * (radius * i)), radius * i);
        }
    }
}