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

public class Enemy : MonoBehaviour
{
    [SerializeField] private FSM _fsm;


    [SerializeField] [Range(0, 360)] float _viewAngle = 90f;
    [SerializeField] float _viewDistance = 5f;
    [SerializeField] float _remainingDistance = 3f;
    [SerializeField] Transform _enemyEye;
    [SerializeField] Transform _target;
    [SerializeField] Light _light;
    [SerializeField] NavMeshAgent _agent;

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
        _target = player.transform;
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

        _observableDetection.Where(td => !td).Subscribe(_ => _fsm.SetState<Patrol>());
        _observableDetection.Where(td => td).Subscribe(_ => _fsm.SetState<Chase>());

        _fsm.AddState(new Idle(_fsm));
        _fsm.AddState(new Patrol(_fsm, _agent, _remainingDistance, this));
        _fsm.AddState(new Chase(_fsm, _agent, _target, this));

        _fsm.SetState<Idle>();
    }

    private void Update()
    {
        _fsm.Update();

        float distanceToPlayer = Vector3.Distance(_target.transform.position, _agent.transform.position);
        if (IsInView() && !TargetDetected)
        {
            TargetDetected = true;
            Chasing().Forget();
        }

        DrawViewState();
    }


    private bool IsInView()
    {
        Vector3 target_v = _target.position - _enemyEye.position;

        float realAngle = Vector3.Angle(_enemyEye.forward, target_v);
        RaycastHit hit;
        if (Physics.Raycast(_enemyEye.transform.position, target_v, out hit,
            _viewDistance))
        {
            if (realAngle < _viewAngle / 2f &&
                Vector3.Distance(_enemyEye.position, _target.position) <= _viewDistance &&
                hit.transform == _target.transform)
                return true;
        }

        return false;
    }

    private void DrawViewState()
    {
        Vector3 left = _enemyEye.position +
                       Quaternion.Euler(new Vector3(0, _viewAngle / 2f, 0)) * (_enemyEye.forward * _viewDistance);
        Vector3 right = _enemyEye.position +
                        Quaternion.Euler(-new Vector3(0, _viewAngle / 2f, 0)) * (_enemyEye.forward * _viewDistance);
        Debug.DrawLine(_enemyEye.position, left, Color.yellow);
        Debug.DrawLine(_enemyEye.position, right, Color.yellow);
    }

    private async UniTask Chasing()
    {
        while (true)
        {
            if (!IsInView())
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
        Gizmos.DrawSphere(_agent.destination, 1);
    }
}