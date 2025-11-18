using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDodge : MonoBehaviour
{
    // When the enemy is hit by a laser, it dodges
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            transform.GetComponentInParent<Enemy>().Dodge();
            Destroy(GetComponent<Collider2D>());
        }
    }
}
