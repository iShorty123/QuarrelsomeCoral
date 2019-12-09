using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    public static bool m_GameOverFlag;


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        GetComponentInChildren<Text>().rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        GetComponentInChildren<Text>().rectTransform.rect.Set(Screen.width / 2, -100, Screen.width, Screen.height);
        m_GameOverFlag = false;

        StartCoroutine(GetInstanceOnceReady());

    }

    private IEnumerator GetInstanceOnceReady()
    {
        while (this == null)
        {
            yield return new WaitForEndOfFrame();
        }
        while (SubmarineManager.GetInstance() == null)
        {
            yield return new WaitForEndOfFrame();
        }
        SubmarineManager.GetInstance().m_GameOverScript = this;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_GameOverFlag)
        {
            Time.timeScale = 0;
            m_GameOverFlag = false;
        }
    }

    public void RestartButton()
    {
        PutThingsBack();
        SceneManager.LoadScene("World_ZB338");
    }

    public void MainMenuButton()
    {
        PutThingsBack();
        SceneManager.LoadScene("MainMenu");
    }

    void PutThingsBack()
    {
        Time.timeScale = 1.0f;
    }
}
