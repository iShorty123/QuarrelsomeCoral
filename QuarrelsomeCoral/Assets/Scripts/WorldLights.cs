using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldLights : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        //Follow sub around so we can have less lights in the world
        transform.position = new Vector3(SubmarineManager.GetInstance().m_Submarine.transform.position.x, 0, 0);
    }
}
