using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : State
{
    private System.Random _rand;
    private Transform _transform;

    protected float distance;
    NavMeshAgent agent;
    Enemy enemy;

    public Patrol(FSM fsm, NavMeshAgent agent, float d, Enemy enemy) : base(fsm)
    {
        _rand = new System.Random();
        _transform = agent.gameObject.transform;

        this.agent = agent;
        distance = d;
        this.enemy = enemy;
    }

    public override void Enter()
    {
        Debug.Log("Patrol [ENTER]");
        agent.speed = 2;
        agent.angularSpeed = 180;
        enemy.Light.GetComponent<Light>().color = Color.yellow;
        enemy.Light.GetComponent<Light>().intensity = 15f;
    }

    public override void Exit()
    {
        Debug.Log("Patrol [EXIT]");
    }

    public override void Update()
    {
        Debug.Log("Patrol [UPDATE]");

        if (agent.remainingDistance <= distance)
        {
            SetDestination(_rand.Next(-9, 9), _rand.Next(-9, 9));
        }
    }

    /// <summary>
    /// y = _transform.position.y
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void SetDestination(float x = 0, float z = 0)
    {
        agent.destination = new Vector3(x, _transform.position.y, z);
    }
}