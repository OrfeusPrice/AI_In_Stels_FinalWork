using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Check : State
{
    NavMeshAgent _agent;
    Enemy _enemy;
    private Enemy _checkableEnemy;

    public Check(FSM fsm, NavMeshAgent agent, Enemy enemy) : base(fsm)
    {
        _agent = agent;
        _enemy = enemy;
    }

    public override void Enter()
    {
        Debug.Log("Check [ENTER]");

        _enemy.Light.GetComponent<Light>().color = Color.yellow;
        BeginCheck().Forget();
    }

    public override void Exit()
    {
        Debug.Log("Check [EXIT]");
        _checkableEnemy = null;
    }

    public override void Update()
    {
        _enemy.TryDetectPlayer();
        Debug.Log("Check [UPDATE]");
    }

    public async UniTask BeginCheck()
    {
        int count = 0;
        while (_checkableEnemy == null)
        {
            count++;
            await UniTask.WaitForSeconds(0.1f);
            if (count >= 100) break;
        }

        if (_checkableEnemy != null)
        {
            await UniTask.WaitForSeconds(3);
            _checkableEnemy?.Disable();
            await UniTask.WaitForSeconds(8f);
            if (_fsm.CurrentState is Check)
                _fsm.SetState<Patrol>();
        }

        else _fsm.SetState<Patrol>();
    }

    public void SetCheckableEnemy(Enemy enemy)
    {
        _checkableEnemy = enemy;
    }
}