using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform BgImage = null;
    public Transform MidImage = null;

    private string m_HorizontalControls;
    private string m_VerticalControls;

    private Rigidbody2D m_SubmarineRigidbody;
    private GameObject m_Submarine;

    float div = 4f;

    // Start is called before the first frame update
    void Start()
    {
        m_SubmarineRigidbody = SubmarineManager.GetInstance().m_Submarine.m_RigidBody;
        m_Submarine = m_SubmarineRigidbody.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = m_SubmarineRigidbody.velocity;

        transform.position = new Vector3(m_Submarine.transform.position.x, m_Submarine.transform.position.y, -10);
        MidImage.transform.Translate(velocity / (2 * div) * Time.deltaTime);
        BgImage.transform.Translate(velocity / div * Time.deltaTime);
    }

}
