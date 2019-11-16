using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableFogOfWar : MonoBehaviour
{

    private GameObject m_FogOfWarCanvas;
    private GameObject m_FogOfWarMainFOV;
    private GameObject m_FogOfWarSecondaryFOV;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FoundYet());

    }

    IEnumerator FoundYet()
    {
        while (GameObject.Find("FogOfWarCanvas") == null)
        {
            yield return new WaitForEndOfFrame();
        }
        m_FogOfWarCanvas = GameObject.Find("FogOfWarCanvas");
        m_FogOfWarMainFOV = GameObject.Find("FogOfWarMainFOV");
        m_FogOfWarSecondaryFOV = GameObject.Find("FogOfWarSecondaryFOV");

        m_FogOfWarCanvas.SetActive(false);
        m_FogOfWarMainFOV.SetActive(false);
        m_FogOfWarSecondaryFOV.SetActive(false);
    }

}
