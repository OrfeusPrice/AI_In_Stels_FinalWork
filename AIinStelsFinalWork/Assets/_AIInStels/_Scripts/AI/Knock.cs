using UnityEngine;
using UnityEngine.AI;
using VLB;

public class Knock : State
{
    NavMeshAgent _agent;
    Enemy _enemy;

    public Knock(FSM fsm, NavMeshAgent agent, Enemy enemy) : base(fsm)
    {
        _agent = agent;
        _enemy = enemy;
    }

    public override void Enter()
    {
        Debug.Log("Knock [ENTER]");

        _agent.destination = _agent.transform.position;
        _agent.gameObject.layer = LayerMask.NameToLayer("Knocked");
        
        _enemy.Light.color = new Color(0.7f, 0.5f, 0.3f);
        _enemy.Light.GetComponent<VolumetricLightBeam>().Reset(); 
    }

    public override void Exit()
    {
        Debug.Log("Knock [EXIT]");
    }

    public override void Update()
    {
        Debug.Log("Knock [UPDATE]");
    }
}