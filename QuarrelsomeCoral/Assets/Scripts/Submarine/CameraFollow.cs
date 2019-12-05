using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform BgImage = null;
    public Transform MidImage = null;
    public Transform Sky = null;
    public Collider2D SkyCollider = null;

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

        SpriteRenderer bgRenderer = BgImage.GetComponent<SpriteRenderer>();
        float rightBorder = bgRenderer.bounds.max.x;
        float leftBorder = bgRenderer.bounds.min.x;

        SpriteRenderer midRenderer = MidImage.GetComponent<SpriteRenderer>();
        rightBorder = midRenderer.bounds.max.x;
        leftBorder = midRenderer.bounds.min.x;

        if ((m_Submarine.transform.position.x - leftBorder) < 75 || (rightBorder - m_Submarine.transform.position.x) < 75)
        {
            //double mid image width
            Vector2 size = midRenderer.size;
            size.x += (float)39.07; //30;
            midRenderer.size = size;

            //double bg image width
            size = bgRenderer.size;
            size.x += (float)39.07; //(float)6.48; //12.96
            bgRenderer.size = size;

            //double sky image width
            size = Sky.transform.localScale;
            size.x += 1f;
            Sky.transform.localScale = size;

            //double sky collider image width
            size = SkyCollider.transform.localScale;
            size.x += 10f;
            SkyCollider.transform.localScale = size;
        }

        if (transform.position.y < -50)
        {
            transform.position = new Vector3(transform.position.x, -50, transform.position.z);
        }

    }

}

