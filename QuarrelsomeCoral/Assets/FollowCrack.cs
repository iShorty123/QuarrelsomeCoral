using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCrack : MonoBehaviour
{
    public GameObject m_MyCrack;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_MyCrack.activeSelf)
        {
            transform.localScale = new Vector3(m_MyCrack.transform.localScale.x +.4f, 1, 1);
        }
        else
        {
            transform.localScale = Vector3.zero;
        }
    }
}
