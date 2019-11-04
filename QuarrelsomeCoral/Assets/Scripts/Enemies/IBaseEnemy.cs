using UnityEngine;

public interface IBaseEnemy
{
    void Move();

    void Attack();

    void HitSubmarine(ContactPoint2D _impactSpot);

    void HitShield(ContactPoint2D _impactSpot);
}
