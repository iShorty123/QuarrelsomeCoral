using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public int m_Player;
    public float m_Speed;
    public float m_SubmarineSpeed;
    public GameObject m_Submarine;

    private bool m_OnLaddder;
    private Rigidbody2D m_RigidBody;
    private Rigidbody2D m_SubmarineRigidBody;
    private SpriteRenderer m_SpriteRenderer;
    private GameObject m_ClosestStation;
    private GameObject m_CurrentStation;
    private bool m_CanMove;
    private bool m_InStation;
    private bool m_IsPilot;
    private bool m_AtTopOfLadder = true;
    private bool m_AtBottomOfLadder;
    private const float m_DISTANCEALLOWEDBETWEENWALLANDPLAYER = 0.3f;
    private Animator m_Animator;

    private string m_Horizontal = "Horizontal_P";
    private string m_Vertical = "Vertical_P";
    private string m_Action1 = "Action1_P";
    private string m_Action2 = "Action2_P";


    enum PlayerState { Walking, TopWeaponStation, BottomWeaponStation, Pilot, ShieldStation, ArmoryStation, RepairStation, MapStation}
    PlayerState m_PlayerState;

    // Start is called before the first frame update
    void Start()
    {
        MapPlayerControls();
        m_PlayerState = PlayerState.Walking;
        m_CanMove = true;
        m_Speed = 5;
        m_OnLaddder = false;
        m_InStation = false;
        m_IsPilot = false;
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SubmarineRigidBody = m_Submarine.GetComponent<SubmarineControl>().m_RigidBody;
        m_ClosestStation = null;
        m_Animator = GetComponent<Animator>();
        m_Animator.SetBool("Move", false);
    }

    // Update is called once per frame
    void Update()
    {
        EnterOrExitStation();
        CheckPlayerActions(m_PlayerState);
    }

    private void FixedUpdate()
    {
        if (m_CanMove)
        {
            MoveCharacter();
        }

        if (m_IsPilot)
        {
            MoveSubmarine();
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
        if (Physics2D.Raycast(transform.position, Vector2.right, m_DISTANCEALLOWEDBETWEENWALLANDPLAYER, layerMask))
        {
            canMoveRight = false;
        }
        else
        {
            canMoveRight = true;
        }
        if (Physics2D.Raycast(transform.localPosition, Vector2.left, m_DISTANCEALLOWEDBETWEENWALLANDPLAYER, layerMask))
        {
            canMoveLeft = false;
        }
        else
        {
            canMoveLeft = true;
        }

        //Horizontally
        float moveHorizontally = Input.GetAxis(m_Horizontal);

        if (m_AtTopOfLadder || m_AtBottomOfLadder )//|| m_OnLaddder)
        {
            if ((moveHorizontally > 0 && !canMoveRight) || (moveHorizontally < 0 && !canMoveLeft)) { moveHorizontally = 0; } //Prevent moving through a wall
            m_RigidBody.velocity = new Vector2(moveHorizontally * m_Speed, m_RigidBody.velocity.y);
        }
        else
        {
            m_RigidBody.velocity = new Vector2(0, -m_Speed); //Free fall down as we move off the ladder before reaching the bottom
        }

        FlipSprite(m_SpriteRenderer, Input.GetAxisRaw(m_Horizontal));       

        //Vertically
        float moveVertically = Input.GetAxis(m_Vertical);
        if (m_OnLaddder)
        {
            if (!m_AtTopOfLadder && !m_AtBottomOfLadder)
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

        if (m_RigidBody.velocity.x != 0)
        {
            m_Animator.SetBool("Move", true);
        }
        else
        {
            m_Animator.SetBool("Move", false);
        }
    }

    private void MoveSubmarine()
    {
        float moveHorizontally = Input.GetAxis(m_Horizontal);
        float moveVertically = Input.GetAxis(m_Vertical);
        //Move submarine via MovePosition(world position translation)
        m_SubmarineRigidBody.MovePosition(new Vector2(m_SubmarineRigidBody.position.x + (moveHorizontally * Time.fixedDeltaTime * m_SubmarineSpeed),
                       m_SubmarineRigidBody.position.y + (moveVertically * Time.fixedDeltaTime * m_SubmarineSpeed)));

    }

    private void EnterOrExitStation()
    {
        if (m_ClosestStation != null && Input.GetButtonDown(m_Action1))
        {
            if (m_InStation) // In -> Out
            {
                m_InStation = false;
                m_CurrentStation.GetComponent<StationController>().SetTutorialText();
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
                    m_CurrentStation.GetComponent<StationController>().SetTutorialText();
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

    private void CheckPlayerActions(PlayerState _playerState) //Is this necessary?
    {
        m_CanMove = false; //As every Player State does not allow the user to move disable here and then re-enable if we are in the Walking State
        m_IsPilot = false; // ""
        switch (_playerState)
        {         
            case PlayerState.Walking:
                m_CanMove = true;
                break;
            case PlayerState.ArmoryStation:
                break;
            case PlayerState.MapStation:
                break;
            case PlayerState.TopWeaponStation:

                GunController.m_RotationAngle += Input.GetAxisRaw(m_Horizontal);
                GunController.m_Fire = Input.GetButton(m_Action2);

                break;
            case PlayerState.BottomWeaponStation:
                break;
            case PlayerState.RepairStation:
                break;
            case PlayerState.ShieldStation:
                break;
            case PlayerState.Pilot:
                m_IsPilot = true;
                break;
            default:
                break;
        }
    }

    private void DetermineStationEnter(string _stationName)
    {
        if (_stationName == "Station_A")
        {
            m_PlayerState = (PlayerState)9; //Sends to default state - does nothing
        }
        else if (_stationName == "Station_B")
        {
            m_PlayerState = (PlayerState)9; //Sends to default state - does nothing
        }
        else if (_stationName == "Station_C")
        {
            m_PlayerState = (PlayerState)9; //Sends to default state - does nothing
        }
        else if(_stationName == "ShieldStation")
        {
            m_PlayerState = PlayerState.ShieldStation;
            ShieldManager.GetShieldManagerInstance().m_PlayerControlled = true;
            ShieldManager.GetShieldManagerInstance().m_PlayerControlScheme = m_Horizontal;
        }
        else if (_stationName == "PilotStation")
        {
            m_PlayerState = PlayerState.Pilot;
        }
        else if (_stationName == "TopWeaponStation")
        {
            m_PlayerState = PlayerState.TopWeaponStation;
        }
    }
    private void DetermineStationExit(string _stationName)
    {
        m_PlayerState = PlayerState.Walking;
        if (_stationName == "Station_A")
        {
            
        }
        else if (_stationName == "Station_B")
        {
           
        }
        else if (_stationName == "Station_C")
        {
           
        }
        else if (_stationName == "ShieldStation")
        {
            ShieldManager.GetShieldManagerInstance().m_PlayerControlled = false;
            ShieldManager.GetShieldManagerInstance().m_PlayerControlScheme = string.Empty;
        }
        else if (_stationName == "PilotStation")
        {

        }
        else if (_stationName == "TopWeaponStation")
        {

        }
    }

}
