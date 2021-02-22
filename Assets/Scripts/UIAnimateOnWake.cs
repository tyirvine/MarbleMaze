using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimateOnWake : MonoBehaviour
{
    // Reference
    public Animator animator;
    public UIManager uiManager;

    /// <summary>Triggers the wake bool for any animator this is attached to.</summary>
    public void SetAnimation(bool open)
    {
        animator.SetBool("Awake", open);
    }

    /// <summary>Closes self.</summary>
    public void CloseSelf()
    {
        uiManager.CreditsMenu(false);
    }
}
