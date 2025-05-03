using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private PlayerAnimator _playerAnimator;

    [SerializeField] private float _speed;
    private Rigidbody _rigidbody;
    private Transform _transform;
    private Camera _mainCamera;
    private CameraController _camController;
    private float _rotationSpeed;

    private void Start()
    {
        _playerAnimator = new PlayerAnimator(_animator);

        _speed = 5;
        _rotationSpeed = 5;
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
        _mainCamera = Camera.main;
        _mainCamera.transform.SetParent(this._transform);
        _mainCamera.transform.localPosition = new Vector3(0, 2, -2);
        _camController = _mainCamera.gameObject.GetComponent<CameraController>();
    }

    public void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");

        transform.Rotate(0f, mouseX * _rotationSpeed, 0f);

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        Vector3 moveDirection = transform.TransformDirection(input) * _speed;
        moveDirection.y = _rigidbody.linearVelocity.y;
        _rigidbody.linearVelocity = moveDirection;

        _playerAnimator.SetAnimation(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (_camController.TryGetEnemy(out Enemy enemy))
            {
                enemy.SetKnock();
            }
        }
    }
}