﻿using UnityEngine;
using UnityEngine.AI;

//[RequireComponent(typeof(NavMeshAgent))]
public class AIStateController : MonoBehaviour
{
    [SerializeField]
    private AIState currentState;

    public NavMeshAgent Agent { get; private set; }

    private AIStateData data;

    private void Awake()
    {
        data = new AIStateData(this);
        Agent = GetComponent<NavMeshAgent>();
        currentState.StartState(this);
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }

    public AIDataContainer GetData(AIStateDataType _type)
    {
        return data.GetData(_type);
    }

    public void Transition(AIState nextState)
    {
        if (nextState != currentState)
        {
            OnStateEnd(currentState);
            currentState = nextState;
            OnStateStart(currentState);
        }
    }

    private void OnStateStart(AIState state)
    {
        state.StartState(this);
    }

    private void OnStateEnd(AIState state)
    {
        state.EndState(this);
    }
}