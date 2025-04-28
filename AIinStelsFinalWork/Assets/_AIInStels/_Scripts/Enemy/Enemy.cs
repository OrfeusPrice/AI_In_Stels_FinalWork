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

public partial class Enemy : MonoBehaviour
{
    [SerializeField] private FSM _fsm;
    private EnemyAnimator _enemyAnimator;

    [SerializeField] private float _remainingDistance = 3f;
    [SerializeField] private Light _light;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _viewSensorTransform;
    [SerializeField] private ViewSensor _viewSensor;

    [SerializeField] private SphereCollider _trigger;


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
        _viewSensor = new ViewSensor(player.transform, _viewSensorTransform, 90, 5);
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

        _observableDetection.Where(td => !td).Subscribe(_ => _fsm.SetState<Patrol>()).AddTo(_compositeDisposable);
        _observableDetection.Where(td => td).Subscribe(_ => _fsm.SetState<Chase>()).AddTo(_compositeDisposable);

        _fsm._observableState.Subscribe(x => _enemyAnimator.SetAnimation(x)).AddTo(_fsm._compositeDisposable);
    }

    private void Update()
    {
        _fsm.Update();

        float distanceToPlayer = Vector3.Distance(_viewSensor.Target.transform.position, _agent.transform.position);
        if (!(_fsm.CurrentState is Knock || _fsm.CurrentState is Idle))
            if (_viewSensor.IsInView() && !TargetDetected ||
                Physics.OverlapSphere(this.transform.position + Vector3.up, 2, LayerMask.GetMask("Player")).Length == 1)
            {
                TargetDetected = true;
                CheckOnChasing().Forget();
            }

        _viewSensor.DrawViewState();
    }

    public void SetKnock()
    {
        Light.color = new Color(1, 0.5f, 0.3f);
        _fsm.SetState<Knock>();
    }


    private async UniTask CheckOnChasing()
    {
        while (_viewSensor != null)
        {
            if (!_viewSensor.IsInView() ||
                Physics.OverlapSphere(this.transform.position, 2, LayerMask.GetMask("Player")).Length == 1)
            {
                await UniTask.WaitForSeconds(0.5f);
                TargetDetected = false;
            }

            await UniTask.WaitForSeconds(0.5f);
            break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_agent.destination, 0.5f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(this.transform.position + Vector3.up, 2);
    }
}