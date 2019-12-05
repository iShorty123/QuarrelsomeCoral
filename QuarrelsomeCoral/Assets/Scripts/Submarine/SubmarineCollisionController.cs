using System.Collections;
using UnityEngine;

public class SubmarineCollisionController : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Check to see if a enemy hit us - if they did, call its HitSubmarine function
        IBaseEnemy enemy = collision.collider.GetComponent<IBaseEnemy>();
        if (enemy != null)
        {
            enemy.HitSubmarine(collision.GetContact(0));
        }
        else if (collision.collider.GetComponent<UnityEngine.Tilemaps.Tilemap>() != null)
        {
            SubmarineManager.GetInstance().m_Submarine.CollidedWithTerrain(collision.GetContact(0));
        }
        else
        {
            Debug.Log("Collided with: " + collision.collider.name);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("Hello: " + collision.name);
    }

}
