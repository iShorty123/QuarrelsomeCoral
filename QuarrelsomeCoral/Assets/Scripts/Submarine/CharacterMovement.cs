using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public int m_Player;
    public float m_Speed;
    public float m_SubmarineSpeed;

    private bool m_OnLaddder;
    private Rigidbody2D m_RigidBody;
    private SpriteRenderer m_SpriteRenderer;
    private GameObject m_ClosestStation;
    private GameObject m_CurrentStation;
    private bool m_CanMove;
    private bool m_InStation;
    private bool m_AtTopOfLadder = true;
    private bool m_AtBottomOfLadder;
    private const float DISTANCE_ALLOWED_BETWEEN_WALL_AND_PLAYER = 0.35f;
    private Animator m_Animator;


    private string m_Horizontal = "Horizontal_P";
    private string m_Vertical = "Vertical_P";
    private string m_Action1 = "Action1_P";
    private string m_Action2 = "Action2_P";

    // Start is called before the first frame update
    void Start()
    {
        MapPlayerControls();


        m_CanMove = true;
        m_Speed = 5;
        m_OnLaddder = false;
        m_InStation = false;
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_ClosestStation = null;
        m_Animator = GetComponent<Animator>();
        m_Animator.SetBool("Move", false);
    }

    // Update is called once per frame
    void Update()
    {
        EnterOrExitStation();
    }

    private void FixedUpdate()
    {
        if (m_CanMove)
        {
            MoveCharacter();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Submarine") // Pushing again sub walls
        {
        }
        else if (collision.name == "FirstFloor")
        {
            m_AtBottomOfLadder = true;
            m_AtTopOfLadder = false;
        }
        else if (collision.name == "SecondFloor")
        {
            m_AtTopOfLadder = true;
            m_AtBottomOfLadder = false;
        }
        else if (collision.tag == "Ladder")
        {
            m_OnLaddder = true;
        }
        
        
        if (collision.tag == "Station")
        {
            m_ClosestStation = collision.gameObject;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name == "FirstFloor")
        {
            m_AtBottomOfLadder = true;
            if (!m_OnLaddder || m_RigidBody.velocity.y < 0)
            {
                m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, 0);//Remove y velocity
            }
        }
        else if (collision.name == "SecondFloor")
        {
            m_AtTopOfLadder = true;
            if (!m_OnLaddder || m_RigidBody.velocity.y > 0)
            {
                m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, 0); //Remove y velocity
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "FirstFloor")
        {
            m_AtBottomOfLadder = false;
        }
        else if (collision.name == "SecondFloor")
        {
            m_AtTopOfLadder = false;
        }
        else if (collision.tag == "Ladder")
        {
            m_OnLaddder = false;
        }
        if (collision.tag == "Station")
        {
            m_ClosestStation = null;
        }
    }


    private void FlipSprite(SpriteRenderer _mySprite, float _direction)
    {
        if (_direction > 0)
        {           
            _mySprite.flipX = false;
        }
        else if (_direction < 0)
        {            
           _mySprite.flipX = true;
        }
    }

    private void MoveCharacter()
    {
        //Determine if against a wall
        int layerMask = 1 << LayerMask.NameToLayer("SubmarineInterior"); //Layer: Submarine Walls
        bool canMoveRight, canMoveLeft;
        if (Physics2D.Raycast(transform.position, Vector2.right, DISTANCE_ALLOWED_BETWEEN_WALL_AND_PLAYER, layerMask))
        {
            canMoveRight = false;
        }
        else
        {
            canMoveRight = true;
        }
        if (Physics2D.Raycast(transform.position, Vector2.left, DISTANCE_ALLOWED_BETWEEN_WALL_AND_PLAYER, layerMask))
        {
            canMoveLeft = false;
        }
        else
        {
            canMoveLeft = true;
        }

        //Horizontally
        float moveHorizontally = Input.GetAxis(m_Horizontal);
        float moveVertically = Input.GetAxis(m_Vertical);
        FlipSprite(m_SpriteRenderer, moveHorizontally);

        //Prevent moving through a wall
        if ((moveHorizontally > 0 && !canMoveRight) || (moveHorizontally < 0 && !canMoveLeft)) { moveHorizontally = 0;}

        if (m_AtTopOfLadder || m_AtBottomOfLadder)
        {
            m_RigidBody.velocity = new Vector2(moveHorizontally * m_Speed, 0);
        }           
        
        if (m_OnLaddder) //Vertically
        {
            if (!m_AtTopOfLadder && !m_AtBottomOfLadder) //If we're only on the ladder, prevent sideways movement
            {
                moveHorizontally = 0;
            }
            if (moveVertically > 0 && !m_AtTopOfLadder) //Want to go up and can go up
            {
                m_RigidBody.velocity = new Vector2(moveHorizontally * m_Speed, (moveVertically *  m_Speed));            
            }
            else if (moveVertically < 0 && !m_AtBottomOfLadder) //Want to go down and can go down
            {
                m_RigidBody.velocity = new Vector2(moveHorizontally * m_Speed, (moveVertically * m_Speed));
            }
            else if (moveVertically == 0) //Prevent player from floating up or down the ladder slowly
            {
                m_RigidBody.velocity = new Vector2(moveHorizontally * m_Speed, (moveVertically * m_Speed));
            }
        }
        else if (!m_AtTopOfLadder && !m_AtBottomOfLadder)
        {
            m_RigidBody.velocity = new Vector2(moveHorizontally * m_Speed, -m_Speed); //Fall down until on the first or second floor
        }

        SetWalkingAnimation();
    }

    private void SetWalkingAnimation()
    {
        if (m_RigidBody.velocity.x != 0)
        {
            m_Animator.SetBool("Move", true);
        }
        else
        {
            m_Animator.SetBool("Move", false);
        }
    }

    private void EnterOrExitStation()
    {
        if (m_ClosestStation != null && Input.GetButtonDown(m_Action1))
        {
            if (m_InStation) // In -> Out
            {
                m_InStation = false;
                DetermineStationExit(m_CurrentStation.name);
                m_CurrentStation = null;                            
            }
            else // Out -> In
            {
                if (!m_ClosestStation.GetComponent<StationController>().m_PlayerControlled) //Can only enter if no one else is already there
                {
                    m_RigidBody.velocity = Vector2.zero;
                    m_InStation = true;
                    m_CurrentStation = m_ClosestStation;
                    DetermineStationEnter(m_CurrentStation.name);
                }
            }
        }
    }

    private void MapPlayerControls()
    {
        m_Horizontal = m_Horizontal + m_Player;
        m_Vertical = m_Vertical + m_Player;
        m_Action1 = m_Action1 + m_Player;
        m_Action2 = m_Action2 + m_Player;
    }

    private void DetermineStationEnter(string _stationName)
    {
        m_CanMove = false;
        m_Animator.SetBool("Move", false);
        switch (_stationName)
        {
            case SubmarineManager.PILOT_STATION:
                SubmarineManager.GetInstance().m_Submarine.SetControls(true, m_Horizontal, m_Vertical);
                break;
            case SubmarineManager.TOP_WEAPON_STATION:
                SubmarineManager.GetInstance().m_TopWeaponStation.SetControls(true, m_Horizontal, m_Action2, -92, 92);
                break;
            case SubmarineManager.BOTTOM_WEAPON_STATION:
                SubmarineManager.GetInstance().m_BottomWeaponStation.SetControls(true, m_Horizontal, m_Action2, 88, 270);
                break;
            case SubmarineManager.SHIELD_STATION:
                SubmarineManager.GetInstance().m_Shield.SetControls(true, m_Horizontal);
                break;
            case SubmarineManager.MAP_STATION:
                SubmarineManager.GetInstance().m_MiniMap.SetControls(true, m_Horizontal, m_Vertical, m_Action2);
                break;
            case SubmarineManager.ARMORY_STATION:
                break;
            default: 
                break;
        }
    }

    private void DetermineStationExit(string _stationName)
    {
        m_CanMove = true;
        switch (_stationName)
        {
            case SubmarineManager.PILOT_STATION:
                SubmarineManager.GetInstance().m_Submarine.SetControls(false, string.Empty, string.Empty);
                break;
            case SubmarineManager.TOP_WEAPON_STATION:
                SubmarineManager.GetInstance().m_TopWeaponStation.SetControls(false, string.Empty, string.Empty, 0, 0);
                break;
            case SubmarineManager.BOTTOM_WEAPON_STATION:
                SubmarineManager.GetInstance().m_BottomWeaponStation.SetControls(false, string.Empty, string.Empty, 0, 0);
                break;
            case SubmarineManager.SHIELD_STATION:
                SubmarineManager.GetInstance().m_Shield.SetControls(false, string.Empty);
                break;
            case SubmarineManager.MAP_STATION:
                SubmarineManager.GetInstance().m_MiniMap.SetControls(false, string.Empty, string.Empty, string.Empty);
                break;
            case SubmarineManager.ARMORY_STATION:
                break;
            default:
                break;
        }
    }



}
