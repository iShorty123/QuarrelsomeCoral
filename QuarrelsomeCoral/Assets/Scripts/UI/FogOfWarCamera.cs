using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FogOfWarCamera : MonoBehaviour
{

    //Class is used to set the two fog of war cameras' size equal to 1/2 this Raw Image's width (or height as both width and height have to be, and are the same)
    //This ensures the fog of war is revealed properly if camera sizes or this Raw Image's size changes at all.
    //Fog of war implemented via the help of https://www.youtube.com/watch?v=MUV9Nr-cIGU

    private GameObject m_MainFogOfWarCamera;
    private GameObject m_SecondaryFogOfWarCamera;

    void Start()
    {
        m_MainFogOfWarCamera = GameObject.Find("FogOfWarMainCamera");
        m_SecondaryFogOfWarCamera = GameObject.Find("FogOfWarSecondaryCamera");

        m_MainFogOfWarCamera.GetComponent<Camera>().orthographicSize = GetComponent<RawImage>().rectTransform.rect.width / 2;
        m_SecondaryFogOfWarCamera.GetComponent<Camera>().orthographicSize = GetComponent<RawImage>().rectTransform.rect.width / 2;        
    }

}
