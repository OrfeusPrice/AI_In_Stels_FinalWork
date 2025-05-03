using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Chase : State
{
    Transform _target;
    NavMeshAgent _agent;
    Enemy _enemy;

    public Chase(FSM fsm, NavMeshAgent agent, Transform target, Enemy enemy) : base(fsm)
    {
        this._agent = agent;
        this._target = target;
        this._enemy = enemy;
    }

    public Vector3 GetAgentDestination()
    {
        return _agent.destination;
    }

    public override void Enter()
    {
        Debug.Log("Chase [ENTER]");
        _agent.destination = _target.position;
        _agent.speed = 10;
        _agent.angularSpeed = 360;
        _enemy.Light.GetComponent<Light>().color = Color.red;
        _enemy.Light.GetComponent<Light>().intensity = 20f;
        
        _enemy.CheckOnChasing().Forget();
    }

    public override void Exit()
    {
        Debug.Log("Chase [EXIT]");
    }

    public override void Update()
    {
        Debug.Log("Chase [UPDATE]");

        if (_target != null)
        _agent.destination = _target.position;
    }
}