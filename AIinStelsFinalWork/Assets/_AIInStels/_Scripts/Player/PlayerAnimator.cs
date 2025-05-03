using UnityEngine;

public class PlayerAnimator
{
    private Animator _animator;

    public PlayerAnimator(Animator anim)
    {
        _animator = anim;
    }

    public void SetAnimation(float vertical, float horizontal)
    {
        if (vertical > 0)
        {
            _animator.SetBool("isFront", true);
            _animator.SetBool("isBack", false);
        }
        else if (vertical < 0)
        {
            _animator.SetBool("isFront", false);
            _animator.SetBool("isBack", true);
        }
        else if (vertical == 0)
        {
            _animator.SetBool("isFront", false);
            _animator.SetBool("isBack", false);
        }

        if (horizontal > 0)
        {
            _animator.SetBool("isRight", true);
            _animator.SetBool("isLeft", false);
        }
        else if (horizontal < 0)
        {
            _animator.SetBool("isRight", false);
            _animator.SetBool("isLeft", true);
        }
        else if (horizontal == 0)
        {
            _animator.SetBool("isRight", false);
            _animator.SetBool("isLeft", false);
        }
    }
}