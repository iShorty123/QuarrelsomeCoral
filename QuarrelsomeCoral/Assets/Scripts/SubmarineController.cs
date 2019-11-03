using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineController : MonoBehaviour
{
    public Rigidbody2D m_RigidBody;
    public float m_Speed;

    private bool m_HasPilot;
    private string m_HorizontalControls;
    private string m_VerticalControls;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = m_RigidBody.position; //Move sub via the SubmarineRigibody object
    }

    private void FixedUpdate()
    {
        if (m_HasPilot)
        {
            MoveSubmarine();
        }
    }

    private void MoveSubmarine()
    {
        float moveHorizontally = Input.GetAxis(m_HorizontalControls);
        float moveVertically = Input.GetAxis(m_VerticalControls);
        //Move submarine via MovePosition(world position translation)
        m_RigidBody.MovePosition(new Vector2(m_RigidBody.position.x + (moveHorizontally * Time.fixedDeltaTime * m_Speed),
                       m_RigidBody.position.y + (moveVertically * Time.fixedDeltaTime * m_Speed)));
    }

    public void SetControls(bool _hasPilot, string _horizontalControls, string _verticalControls)
    {
        m_HasPilot = _hasPilot;
        m_HorizontalControls = _horizontalControls;
        m_VerticalControls = _verticalControls;
    }


}
