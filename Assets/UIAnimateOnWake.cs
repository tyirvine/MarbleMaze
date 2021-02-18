using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimateOnWake : MonoBehaviour
{
    // Reference
    public Animator animator;

    public void SetAnimation(bool open)
    {
        animator.SetBool("Awake", open);
    }
}
