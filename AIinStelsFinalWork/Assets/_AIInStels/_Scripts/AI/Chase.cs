using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using VLB;

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
        _enemy.Light.color = Color.red;
        _enemy.Light.GetComponent<VolumetricLightBeam>().Reset(); 
        
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