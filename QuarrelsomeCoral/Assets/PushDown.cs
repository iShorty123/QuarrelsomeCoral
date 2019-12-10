using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushDown : MonoBehaviour
{

    Rigidbody2D m_Rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = transform.GetComponent<Rigidbody2D>();
    }


}
