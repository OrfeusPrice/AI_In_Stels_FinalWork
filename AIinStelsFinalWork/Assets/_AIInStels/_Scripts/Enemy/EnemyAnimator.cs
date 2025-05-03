using ModestTree;
using NUnit.Framework;
using UnityEngine;

public class EnemyAnimator
{
    private Animator _animator;
    private string _isChase = "isChase";
    private string _isKnocked = "isKnocked";
    private string _isCheck = "isCheck";
    private string _isIdle = "isIdle";

    public EnemyAnimator(Animator animator)
    {
        _animator = animator;
    }

    public void SetAnimation(State state)
    {
        if (!(state is Idle))
        {
            _animator.SetBool(_isIdle, false);
        }

        if (state is Chase)
        {
            _animator.SetBool(_isChase, true);
            _animator.SetBool(_isKnocked, false);
            _animator.SetBool(_isCheck, false);
        }

        else if (state is Check)
        {
            _animator.SetBool(_isChase, false);
            _animator.SetBool(_isKnocked, false);
            _animator.SetBool(_isCheck, true);
        }

        else if (state is Knock)
        {
            _animator.SetBool(_isChase, false);
            _animator.SetBool(_isKnocked, true);
            _animator.SetBool(_isCheck, false);
        }
        else
        {
            _animator.SetBool(_isChase, false);
            _animator.SetBool(_isKnocked, false);
            _animator.SetBool(_isCheck, false);
        }
    }
}