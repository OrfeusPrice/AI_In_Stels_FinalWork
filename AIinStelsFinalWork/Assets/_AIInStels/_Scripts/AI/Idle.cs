using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : State
{
    public Idle(FSM fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Idle [ENTER]");
    }

    public override void Exit()
    {
        Debug.Log("Idle [EXIT]");
    }

    public override void Update()
    {
        Debug.Log("Idle [UPDATE]");

        if (Input.GetKeyDown(KeyCode.Mouse0)) 
        {
            _fsm.SetState<Patrol>();
        }
    }

}
