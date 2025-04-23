using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using Random = System.Random;

public class FSM
{
    private Random _rand;
    public State current_state { get; set; }

    private Dictionary<Type, State> states = new Dictionary<Type, State>();

    public FSM()
    {
        _rand = new Random();
    }

    public void AddState(State state)
    {
        states.Add(state.GetType(), state);
    }

    public void SetState<T>() where T : State
    {
        var type = typeof(T);

        if (current_state != null && current_state.GetType() == type)
            return;

        if (states.TryGetValue(type, out var newState))
        {
            if (newState.GetType() == typeof(Patrol))
                if (current_state.GetType() == typeof(Idle))
                    (newState as Patrol).SetDestination(_rand.Next(-9, 9), _rand.Next(-9, 9));
                else if (current_state.GetType() == typeof(Chase))
                    (newState as Patrol).SetDestination((current_state as Chase).GetAgentDestination().x,
                        (current_state as Chase).GetAgentDestination().z);

            current_state?.Exit();
            current_state = newState;
            current_state.Enter();
        }
    }

    public void Update()
    {
        current_state.Update();
    }
}