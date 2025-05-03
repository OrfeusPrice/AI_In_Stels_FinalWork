using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Zenject;
using Unit = R3.Unit;

public partial class Enemy : MonoBehaviour
{
    [SerializeField] private FSM _fsm;
    private EnemyAnimator _enemyAnimator;

    [SerializeField] private GameObject _particles;

    [SerializeField] private float _remainingDistance = 3f;
    [SerializeField] private Light _light;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _viewSensorTransform;
    [SerializeField] private ViewSensor _viewSensor;
    private Enemy _checkableEnemy;

    private ReactiveProperty<bool> _isTargetDetected;
    private Observable<bool> _observableDetection => _isTargetDetected;
    private CompositeDisposable _compositeDisposable;

    public Light Light
    {
        get => _light;
        set => _light = value;
    }

    public bool TargetDetected
    {
        get => _isTargetDetected.Value;
        set => _isTargetDetected.Value = value;
    }

    [Inject]
    public void Construct(Player player)
    {
        _viewSensor = new ViewSensor(player.transform, _viewSensorTransform, 90, 10);
    }

    private void OnDisable()
    {
        _compositeDisposable.Dispose();
        _fsm._compositeDisposable.Dispose();
    }

    private void Start()
    {
        _enemyAnimator = new EnemyAnimator(GetComponent<Animator>());

        _fsm = new FSM(_agent, _remainingDistance, _viewSensor, this);
        _isTargetDetected = new ReactiveProperty<bool>(false);
        _compositeDisposable = new CompositeDisposable();

        _observableDetection.Where(td => !td).Skip(1).Subscribe(_ => _fsm.SetState<Patrol>())
            .AddTo(_compositeDisposable);
        _observableDetection.Where(td => td).Subscribe(_ => _fsm.SetState<Chase>()).AddTo(_compositeDisposable);

        _fsm._observableState.Subscribe(x => _enemyAnimator.SetAnimation(x)).AddTo(_fsm._compositeDisposable);
    }

    private void Update()
    {
        _fsm.Update();

        _viewSensor.DrawViewState();
    }

    public async UniTask Disable()
    {
        _particles.SetActive(true);
        await UniTask.WaitForSeconds(2f);
        this.gameObject.SetActive(false);
    }

    public void TryDetectKnockedEnemy()
    {
        if (_viewSensor.IsKnockedEnemyInView(out _checkableEnemy))
        {
            if (_agent.destination != _checkableEnemy.transform.position)
                _agent.destination = _checkableEnemy.transform.position;
            else if (_agent.remainingDistance < 3)
            {
                SetCheck(_checkableEnemy);
            }
        }
    }

    public void TryDetectPlayer()
    {
        if (_viewSensor.IsPlayerInView() && !TargetDetected ||
            Physics.OverlapSphere(this.transform.position + Vector3.up, 2, LayerMask.GetMask("Player")).Length == 1)
        {
            TargetDetected = true;
        }
    }

    public void SetCheck(Enemy enemy)
    {
        _fsm.SetState<Check>();
        if (_fsm.CurrentState is Check)
        {
            (_fsm.CurrentState as Check).SetCheckableEnemy(enemy);
            _viewSensor.SensorPowerUp();
        }

        _checkableEnemy = null;
    }

    public void SetKnock()
    {
        _fsm.SetState<Knock>();
    }


    public async UniTask CheckOnChasing()
    {
        while (_fsm.CurrentState is Chase)
        {
            if (!IsDetectPlayerWhileChase())
            {
                await UniTask.WaitForSeconds(0.5f);
                TargetDetected = false;
            }

            await UniTask.WaitForSeconds(0.5f);
        }
    }

    public bool IsDetectPlayerWhileChase()
    {
        if (_viewSensor != null && this != null)
            return _viewSensor.IsPlayerInView() ||
                   Physics.OverlapSphere(this.transform.position, 2, LayerMask.GetMask("Player")).Length == 1;
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_agent.destination, 0.5f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(this.transform.position + Vector3.up, 2);
    }
}