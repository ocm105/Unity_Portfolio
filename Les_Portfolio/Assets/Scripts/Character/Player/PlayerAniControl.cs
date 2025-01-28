using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAniControl : MonoBehaviour
{
    private PlayerInfo playerInfo;
    private Animator animator;

    public PlayerAniState playerAniState { get; private set; }

    private void Awake()
    {
        playerInfo = this.GetComponent<PlayerInfo>();
    }

    public void SetAnimator(Animator _animator)
    {
        animator = _animator;
    }

    public void AnimationChanger(PlayerAniState state)
    {
        switch (state)
        {
            case PlayerAniState.Default:
                animator.CrossFade("Move_Blend", 0.1f, 0);
                break;
            case PlayerAniState.Jump:
                animator.CrossFade(state.ToString(), 0.1f, 0);
                break;
        }

        playerAniState = state;
    }

    public void SetMoveValue(float value)
    {
        animator.SetFloat("Move", value);
    }
    private IEnumerator AnimationCoroutine(string key)
    {
        if (playerAniState == PlayerAniState.Default)
        {
            animator.CrossFade(key.ToString(), 0.1f, 0);

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(key));
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f);

            AnimationChanger(PlayerAniState.Default);
        }
        yield break;
    }
}
