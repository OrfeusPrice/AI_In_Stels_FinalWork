using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using R3;
using Unity.VisualScripting;
using UnityEngine.AI;
using Random = System.Random;

public class FSM
{
    private Random _rand;

    private ReactiveProperty<State> _currentState;
    public Observable<State> _observableState => _currentState;
    public CompositeDisposable _compositeDisposable;

    public State CurrentState
    {
        get => _currentState.Value;
        set => _currentState.Value = value;
    }

    private Dictionary<Type, State> states = new Dictionary<Type, State>();

    public FSM(NavMeshAgent agent, float remainingDistance, ViewSensor viewSensor, Enemy enemy)
    {
        _rand = new Random();
        _currentState = new ReactiveProperty<State>(new Idle(this));
        _compositeDisposable = new CompositeDisposable();
        

        AddState(new Idle(this));
        AddState(new Patrol(this, agent, remainingDistance, enemy));
        AddState(new Chase(this, agent, viewSensor.Target, enemy));
        AddState(new Knock(this, agent, enemy));
        AddState(new Check(this, agent, enemy));

        SetState<Idle>();
    }

    public void AddState(State state) =>
        states.Add(state.GetType(), state);
    

    public void SetState<T>() where T : State
    {
        var type = typeof(T);

        if (CurrentState != null && CurrentState.GetType() == type)
            return;

        if (states.TryGetValue(type, out var newState))
        {
            if (newState.GetType() == typeof(Patrol))
                if (CurrentState.GetType() == typeof(Chase))
                    (newState as Patrol).SetDestination((CurrentState as Chase).GetAgentDestination().x,
                        (CurrentState as Chase).GetAgentDestination().z);
                else
                    (newState as Patrol).SetDestination(_rand.Next(-9, 9), _rand.Next(-9, 9));

            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }

    public void Update()
    {
        CurrentState.Update();
    }
}