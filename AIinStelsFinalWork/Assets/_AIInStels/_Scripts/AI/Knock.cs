using UnityEngine;

public class Knock : State
{
	public Knock(FSM fsm) : base(fsm)
	{
	}

	public override void Enter()
	{
		Debug.Log("Knock [ENTER]");
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