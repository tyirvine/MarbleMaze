
using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    /// <summary>Just a class for each menu to use. Designated for UIs only.</summary>
    [Serializable]
    public class UIMenu
    {
        public GameObject canvas;
        bool isActive { get => canvas.activeInHierarchy; }

        // Constructor
        public UIMenu(bool isActive)
        {
            this.canvas.SetActive(false);
        }
    }

    /* ------------------------------ Menu Control ----------------------------- */
    public UIMenu startMenu = new UIMenu(true);
    public UIMenu gameoverMenu;
    public UIMenu pauseMenu;
    public UIMenu creditsMenu;
    public UIMenu optionsMenu;

    /// <summary>Opens up a specified menu object.</summary>
    public void MenuControl(UIMenu menu, bool state) => menu.canvas.SetActive(state);

    // Start menu
    public void StartMenu(bool state)
    {
        MenuControl(startMenu, state);
    }

    // Options menu
    public void OptionsMenu(bool state)
    {
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
        MenuControl(pauseMenu, state);
    }

    // Credits menu
    public void CreditsMenu(bool state)
    {
        MenuControl(creditsMenu, state);
    }

    /* ------------------------------ Miscellaneous ----------------------------- */

}
