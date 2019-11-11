using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform BgImage = null;
    public Transform MidImage = null;
    public Transform Sky = null;

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

        if ((m_Submarine.transform.position.x - leftBorder) < 75 || (rightBorder - m_Submarine.transform.position.x) < 75)
        {
            Vector2 size = bgRenderer.size;
            size.x += (float)6.48; //12.96
            bgRenderer.size = size;
        }

        SpriteRenderer midRenderer = MidImage.GetComponent<SpriteRenderer>();
        rightBorder = midRenderer.bounds.max.x;
        leftBorder = midRenderer.bounds.min.x;

        if ((m_Submarine.transform.position.x - leftBorder) < 75 || (rightBorder - m_Submarine.transform.position.x) < 75)
        {
            Vector2 size = midRenderer.size;
            size.x += 12;
            midRenderer.size = size;

            Vector3 skySize = Sky.transform.localScale;
            skySize.x += 0.3f;
            Sky.transform.localScale = skySize;
        }
    }

}

