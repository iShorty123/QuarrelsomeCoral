using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseEnemy
{
    void Move();

    void Attack();

    void TakeDamage();

    void HitSubmarine(ContactPoint2D _impactSpot);
}
