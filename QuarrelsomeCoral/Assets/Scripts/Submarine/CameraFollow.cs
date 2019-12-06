using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform BgImage = null;
    public Transform MidImage = null;
    public Transform BgImageTop = null;
    public Transform MidImageTop = null;
    public Transform Sky = null;
    public Collider2D SkyCollider = null;
    public GameObject waves = null;

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

        Animator anim = waves.GetComponent<Animator>();
        anim.speed = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = m_SubmarineRigidbody.velocity;

        transform.position = new Vector3(m_Submarine.transform.position.x, m_Submarine.transform.position.y, -10);
        MidImage.transform.Translate(velocity / (2 * div) * Time.deltaTime);
        BgImage.transform.Translate(velocity / div * Time.deltaTime);

        SpriteRenderer bgRenderer = BgImage.GetComponent<SpriteRenderer>();
        SpriteRenderer bgTopRenderer = BgImageTop.GetComponent<SpriteRenderer>();
        SpriteRenderer wavesRenderer = waves.transform.GetComponent<SpriteRenderer>();
        SpriteRenderer midRenderer = MidImage.GetComponent<SpriteRenderer>();
        SpriteRenderer midTopRenderer = MidImageTop.GetComponent<SpriteRenderer>();
        SpriteRenderer skyRenderer = Sky.GetComponent<SpriteRenderer>();

        float rightBorder = skyRenderer.bounds.max.x;
        float leftBorder = skyRenderer.bounds.min.x;

        if ((m_Submarine.transform.position.x - leftBorder) < 75 || (rightBorder - m_Submarine.transform.position.x) < 75)
        {
            //double mid image width
            Vector2 size = midRenderer.size;
            size.x += (float)43.89; //30;
            midRenderer.size = size;
            //78.89

            size = midTopRenderer.size;
            size.x += (float)44.05; 
            midTopRenderer.size = size;
            //79.05

            //double bg image width
            size = bgRenderer.size;
            size.x += (float)52.1; //(float)6.48; //12.96
            bgRenderer.size = size;
            //87.1

            //double top bg image width
            size = bgTopRenderer.size;
            size.x += (float)52.1; 
            bgTopRenderer.size = size;

            //double waves image width
            size = wavesRenderer.size;
            size.x += (float)200;
            wavesRenderer.size = size;

            //double sky image width
            size = skyRenderer.size;
            size.x += 105.87f;
            skyRenderer.size = size;
            //205.87

            //double sky collider image width
            size = SkyCollider.transform.localScale;
            size.x += 50f;
            SkyCollider.transform.localScale = size;
        }

        if (transform.position.y < -50)
        {
            transform.position = new Vector3(transform.position.x, -50, transform.position.z);
        }

    }

}

