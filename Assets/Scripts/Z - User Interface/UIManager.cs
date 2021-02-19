﻿using System.Collections;

using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    /// <summary>Just a class for each menu to use. Designated for UIs only.</summary>
    [Serializable]
    public class UIMenu
    {
        public Canvas canvas;
        public GameObject shade;
        public GameObject scaler;
        [HideInInspector] public bool state = false;
    }

    /* ------------------------------- References ------------------------------- */
    [HideInInspector] public PlayerInput playerInput;

    /* ------------------------------ Menu Control ----------------------------- */
    public UIMenu statsMenu;
    public UIMenu startMenu;
    public UIMenu gameoverMenu;
    public UIMenu pauseMenu;
    public UIMenu creditsMenu;
    public UIMenu optionsMenu;

    List<UIMenu> menus = new List<UIMenu>();

    // Find references
    private void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
    }

    /// <summary>Used to delay the opening of start menu.</summary>
    IEnumerator OpenStartMenu()
    {
        yield return new WaitForSeconds(2f);
        StartMenu(true);
    }

    private void Start()
    {
        IEnumerator coroutine = OpenStartMenu();
        StartCoroutine(coroutine);
    }

    /// <summary>Sets state for menu. Used for delaying menu open or close.</summary>
    public IEnumerator DelayMenuControl(UIMenu menu, bool state)
    {
        yield return new WaitForSeconds(0.1f);
        menu.canvas.enabled = state;
    }

    /// <summary>Opens up a specified menu object.</summary>
    public void MenuControl(UIMenu menu, bool state)
    {
        // Animate
        if (menu.shade != null || menu.scaler != null)
        {
            // Setup coroutine
            IEnumerator coroutine = DelayMenuControl(menu, state);
            // Open menu
            if (state)
                menu.canvas.enabled = state;
            else
                StartCoroutine(coroutine);

            // Animate
            if (menu.shade != null) menu.shade.GetComponent<UIAnimateOnWake>().SetAnimation(state);
            if (menu.scaler != null) menu.scaler.GetComponent<UIAnimateOnWake>().SetAnimation(state);
        }
        // Nothing to animate
        else
            menu.canvas.enabled = state;
    }

    // Start menu
    public void StartMenu(bool state)
    {
        startMenu.state = state;
        MenuControl(startMenu, state);
    }

    // Options menu
    public void OptionsMenu(bool state)
    {
        playerInput.PauseControls(state);
        MenuControl(optionsMenu, state);
    }

    // Game over menu
    public void GameoverMenu(bool state)
    {
        MenuControl(gameoverMenu, state);
    }

    // Pause menu
    public void PauseMenu(bool state)
    {
        // Freeze time
        if (state)
        {
            pauseMenu.state = true;
            Time.timeScale = 0f;
        }
        // Unfreeze time
        else
        {
            pauseMenu.state = false;
            Time.timeScale = 1f;
        }
        MenuControl(pauseMenu, state);
    }

    // Credits menu
    public void CreditsMenu(bool state)
    {
        MenuControl(creditsMenu, state);
    }

    // Game view menu
    public void StatsMenu(bool state)
    {
        MenuControl(statsMenu, state);
    }

    /* ------------------------------ Miscellaneous ----------------------------- */

}
