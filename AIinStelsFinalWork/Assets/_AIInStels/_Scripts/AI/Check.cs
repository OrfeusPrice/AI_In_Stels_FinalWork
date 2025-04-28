using UnityEngine;

public class Check : State
{
    public Check(FSM fsm) : base(fsm)
    {
    }


    public override void Enter()
    {
        Debug.Log("Check [ENTER]");
    }

    public override void Exit()
    {
        Debug.Log("Check [EXIT]");
    }

    public override void Update()
    {
        Debug.Log("Check [UPDATE]");
    }
}