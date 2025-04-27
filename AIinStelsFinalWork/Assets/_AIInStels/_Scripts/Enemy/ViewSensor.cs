using System;
using UnityEngine;

[Serializable]
public class ViewSensor
{
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _enemyEye;
    [SerializeField] [Range(0, 360)] private float _viewAngle = 90f;
    [SerializeField] private float _viewDistance = 5f;

    public Transform Target
    {
        get => _target;
        private set => _target = value;
    }

    public ViewSensor(Transform target, Transform enemyEye, float viewAngle, float viewDistance)
    {
        _target = target;
        _enemyEye = enemyEye;
        _viewAngle = viewAngle;
        _viewDistance = viewDistance;
    }


    public bool IsInView()
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

    public void DrawViewState()
    {
        Vector3 left = _enemyEye.position +
                       Quaternion.Euler(new Vector3(0, _viewAngle / 2f, 0)) * (_enemyEye.forward * _viewDistance);
        Vector3 right = _enemyEye.position +
                        Quaternion.Euler(-new Vector3(0, _viewAngle / 2f, 0)) * (_enemyEye.forward * _viewDistance);
        Debug.DrawLine(_enemyEye.position, left, Color.yellow);
        Debug.DrawLine(_enemyEye.position, right, Color.yellow);
    }
}