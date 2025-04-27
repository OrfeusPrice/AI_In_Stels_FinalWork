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
    }

    private void Start()
    {
        _fsm = new FSM();
        _isTargetDetected = new ReactiveProperty<bool>(false);
        _compositeDisposable = new CompositeDisposable();

        _observableDetection.Where(td => !td).Subscribe(_ => _fsm.SetState<Patrol>()).AddTo(_compositeDisposable);
        _observableDetection.Where(td => td).Subscribe(_ => _fsm.SetState<Chase>()).AddTo(_compositeDisposable);

        _fsm.AddState(new Idle(_fsm));
        _fsm.AddState(new Patrol(_fsm, _agent, _remainingDistance, this));
        _fsm.AddState(new Chase(_fsm, _agent, _viewSensor.Target, this));

        _fsm.SetState<Idle>();
    }

    private void Update()
    {
        _fsm.Update();

        float distanceToPlayer = Vector3.Distance(_viewSensor.Target.transform.position, _agent.transform.position);
        if (_viewSensor.IsInView() && !TargetDetected ||
            Physics.OverlapSphere(this.transform.position + Vector3.up, 2, LayerMask.GetMask("Player")).Length == 1)
        {
            TargetDetected = true;
            Chasing().Forget();
        }

        _viewSensor.DrawViewState();
    }

    public void SetIdle()
    {
        Light.color = Color.magenta;
        _fsm.SetState<Idle>();
    }


    private async UniTask Chasing()
    {
        while (true)
        {
            if (!_viewSensor.IsInView() ||
                Physics.OverlapSphere(this.transform.position, 2, LayerMask.GetMask("Player")).Length == 1)
            {
                await UniTask.WaitForSeconds(0.5f);
                TargetDetected = false;
            }

            await UniTask.WaitForSeconds(0.5f);
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