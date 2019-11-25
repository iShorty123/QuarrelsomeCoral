using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject PauseMenu = null;
    public GameObject PauseButton = null;

    public Camera MainCamera = null;
    public GameObject PauseBG = null;

    GameObject submarine = null;

    private void Start()
    {
        if (PauseMenu != null) {
            PauseMenu.SetActive(false);
        }
    }

    private void Update()
    {
        if (PauseBG != null ) {
            Vector3 bgPosition = MainCamera.transform.position;
            bgPosition.x += 0.2f;
            bgPosition.y -= 1;
            bgPosition.z = 100;
            PauseBG.transform.position = bgPosition;
        }
    }

    public void GotoPlayScene()
    {
        //show a spinner
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

    public void GotoCreditsScene()
    {
        SceneManager.LoadScene("Credits");
    }

    public void GotoQuitScene()
    {
        Application.Quit();
    }

    public void GotoPauseScene()
    {
        if (PauseMenu != null) {

            PauseMenu.SetActive(true);
            PauseButton.SetActive(false);
            Time.timeScale = 0;
            submarine = GameObject.Find("SubmarineManager(Clone)");
            recDisabler(submarine.transform);
        }

    }

    public void GotoResumeScene()
    {
        if (PauseMenu != null)
        {
            PauseMenu.SetActive(false);
            PauseButton.SetActive(true);
            Time.timeScale = 1.0f;
            recEnabler(submarine.transform);
        }
    }

    void PutThingsBack() {
        if (PauseMenu != null)
        {
            Time.timeScale = 1.0f;
            submarine.SetActive(false);
            recEnabler(submarine.transform);
        }
    }

    public void GotoReloadScene()
    {
        PutThingsBack();
        GotoPlayScene();
    }

    public void GotoMenuScene()
    {
        PutThingsBack();
        SceneManager.LoadScene("MainMenu");
    }

    private void recDisabler(Transform t)
    {
        if (t.childCount > 0)
        {
            foreach (Transform child in t)
            {
                recDisabler(child);
            }
        }
        Renderer r = t.gameObject.GetComponent<Renderer>();
        Canvas c = t.gameObject.GetComponent<Canvas>();
        if (r != null) r.enabled = false;
        if (c != null) c.enabled = false;
    }

    private void recEnabler(Transform t)
    {
        if (t.childCount > 0)
        {
            foreach (Transform child in t)
            {
                recEnabler(child);
            }
        }
        Renderer r = t.gameObject.GetComponent<Renderer>();
        Canvas c = t.gameObject.GetComponent<Canvas>();
        if (r != null) r.enabled = true;
        if (c != null) c.enabled = false;
    }
}
