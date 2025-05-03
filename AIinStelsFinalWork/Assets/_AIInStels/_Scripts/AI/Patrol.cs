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
    NavMeshAgent _agent;
    Enemy _enemy;

    public Patrol(FSM fsm, NavMeshAgent agent, float d, Enemy enemy) : base(fsm)
    {
        _rand = new System.Random();
        _transform = agent.gameObject.transform;

        this._agent = agent;
        distance = d;
        this._enemy = enemy;
    }

    public override void Enter()
    {
        Debug.Log("Patrol [ENTER]");
        _agent.speed = 2;
        _agent.angularSpeed = 180;
        _enemy.Light.color = new Color(0.6f, 1.6f, 1.0f);
        _enemy.Light.GetComponent<Light>().intensity = 15f;
    }

    public override void Exit()
    {
        Debug.Log("Patrol [EXIT]");
    }

    public override void Update()
    {
        Debug.Log("Patrol [UPDATE]");

        _enemy.TryDetectPlayer();
        _enemy.TryDetectKnockedEnemy();

        if (_agent.remainingDistance <= distance)
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
        if (_transform != null)
            _agent.destination = new Vector3(x, _transform.position.y, z);
    }
}