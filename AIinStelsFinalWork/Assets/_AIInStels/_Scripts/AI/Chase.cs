using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Chase : State
{
    Transform target;
    NavMeshAgent agent;
    Enemy enemy;

    public Chase(FSM fsm, NavMeshAgent agent, Transform target, Enemy enemy) : base(fsm)
    {
        this.agent = agent;
        this.target = target;
        this.enemy = enemy;
    }

    public Vector3 GetAgentDestination()
    {
        return agent.destination;
    }

    public override void Enter()
    {
        Debug.Log("Chase [ENTER]");
        agent.destination = target.position;
        agent.speed = 10;
        agent.angularSpeed = 360;
        enemy.Light.GetComponent<Light>().color = Color.red;
        enemy.Light.GetComponent<Light>().intensity = 20f;
    }

    public override void Exit()
    {
        Debug.Log("Chase [EXIT]");
    }

    public override void Update()
    {
        Debug.Log("Chase [UPDATE]");

        agent.destination = target.position;
    }
}