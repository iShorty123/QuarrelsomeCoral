using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointOfInterestBehavior : MonoBehaviour
{

    private bool m_Capturing;
    private float m_CaptureTime;
    private Image m_ProgressBar;
    private bool m_SpawnedBoss;

    // Start is called before the first frame update
    void Start()
    {
        m_SpawnedBoss = false;
        GetComponentInChildren<Canvas>().worldCamera = Camera.main;
        m_CaptureTime = 5;
        m_ProgressBar = GetComponentInChildren<Canvas>().transform.GetChild(0).transform.GetChild(0).GetComponentInChildren<Image>();
        m_ProgressBar.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Capturing)
        {           
            m_ProgressBar.fillAmount += 1.0f / m_CaptureTime * Time.deltaTime;
            //Debug.Log(m_ProgressBar.fillAmount);
        }
        else
        {
            m_ProgressBar.fillAmount -= 1.0f / m_CaptureTime * Time.deltaTime;
            m_ProgressBar.fillAmount = Mathf.Clamp(m_ProgressBar.fillAmount, 0, 1);
        }
        if (m_ProgressBar.fillAmount >= 1)
        {
            Destroy(gameObject);
        }
        else if (m_ProgressBar.fillAmount >= .5f && !m_SpawnedBoss)
        {
            m_SpawnedBoss = true;
            MainController.GetInstance().RBoss.m_CanSpawn = m_SpawnedBoss;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "SubmarineRigidbody")
        {
            Debug.Log("Entered: " + transform.name + " " + transform.localPosition);
            Debug.Log(m_ProgressBar.transform);
            m_Capturing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "SubmarineRigidbody")
        {
            m_Capturing = false;
        }
    }

    private void OnDestroy()
    {
        if (SubmarineManager.GetInstance() != null)
        {
            SubmarineManager.GetInstance().m_Score += 1000;
        }
    }

}
