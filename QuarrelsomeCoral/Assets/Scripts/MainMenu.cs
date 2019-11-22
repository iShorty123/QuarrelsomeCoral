using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void GotoPlayScene()
    {
        SceneManager.LoadScene("World_ZB338");
    }

    public void GotoTutorialScene()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void GotSettingsScene()
    {
        SceneManager.LoadScene("Settings");
    }

    public void GotoQuitScene()
    {
        Application.Quit();
    }

    public void GotoPauseScene()
    {
        SceneManager.LoadScene("Pause");
    }

    public void GotoMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
