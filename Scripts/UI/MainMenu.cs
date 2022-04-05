using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static void StartGame() {
        SceneManager.LoadScene("TestScene", LoadSceneMode.Single);
    }

    public static void ExitGame() {
        Application.Quit();
    }

    public static void LoadMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public static void OpenMenuPrompt(GameObject prompt) {
        if (prompt.activeSelf) {
            prompt.SetActive(false);
            Time.timeScale = 1;
            InventoryMenu.inMenu = false;
        } else {
            prompt.SetActive(true);
            Time.timeScale = 0;
            InventoryMenu.inMenu = true;
        }
    }
}
