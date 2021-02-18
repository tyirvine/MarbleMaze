
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
        public bool isActiveOnStart = false;
        public bool state = false;

        // Constructor
        public void CanvasSet() => canvas.enabled = isActiveOnStart;
    }

    /* ------------------------------- References ------------------------------- */
    public PlayerInput playerInput;

    /* ------------------------------ Menu Control ----------------------------- */
    public UIMenu gameMenu;
    public UIMenu startMenu;
    public UIMenu gameoverMenu;
    public UIMenu pauseMenu;
    public UIMenu creditsMenu;
    public UIMenu optionsMenu;

    List<UIMenu> menus = new List<UIMenu>();

    // Disable all menus
    private void Awake()
    {
        // Initialize menus
        menus = new List<UIMenu>{
            startMenu,
            gameoverMenu,
            pauseMenu,
            creditsMenu,
            optionsMenu
        };

        foreach (UIMenu menu in menus)
        {
            menu.CanvasSet();
        }

        // Find references
        playerInput = FindObjectOfType<PlayerInput>();
    }

    /// <summary>Opens up a specified menu object.</summary>
    public void MenuControl(UIMenu menu, bool state) => menu.canvas.enabled = state;

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
    public void GameMenu(bool state)
    {
        MenuControl(gameMenu, state);
    }

    /* ------------------------------ Miscellaneous ----------------------------- */

}
