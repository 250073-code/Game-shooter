using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoomingLaser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f; // Laser movement speed
    private float _rotateSpeed = 360f; // How fast the laser can rotate towards target
    private GameObject _closeEnemy; // Closest enemy to home in on

    // Update is called once per frame
    void Update()
    {
        // If no target, find the closest enemy
        if (_closeEnemy == null)
        {
            _closeEnemy = WhereIsEnemy();
        }

        // If a target exists, home in on it
        if (_closeEnemy != null)
        {
            MoveTowardsEnemy();
        }
        else
        {
            // Move straight if no target
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
            // Destroy if out of bounds
            if (transform.position.y > 8f || transform.position.y < -8f || transform.position.x > 11f || transform.position.x < -11f)
            {
                if (transform.parent != null)
                {
                    Destroy(transform.parent.gameObject);
                }
                Destroy(this.gameObject);
            }
        }
    }
    
    // Finds and returns the closest enemy GameObject
    private GameObject WhereIsEnemy()
    {
        try
        {
            GameObject[] enemies;
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float distance = Mathf.Infinity;
            GameObject closestEnemy = null;
            Vector3 position = transform.position;

            foreach (GameObject enemy in enemies)
            {
                Vector3 other = enemy.transform.position - position;
                float currentDistance = other.sqrMagnitude;
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    closestEnemy = enemy;
                }
            }
            return closestEnemy;
        }
        catch
        {
            return null;
        }
    }

    // Rotates and moves the laser towards the closest enemy
    private void MoveTowardsEnemy()
    {
        Vector3 direction = _closeEnemy.transform.position - transform.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        transform.Rotate(0, 0, -rotateAmount * _rotateSpeed * Time.deltaTime);
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
    }

    // Handles collision with enemies: destroys both laser and enemy
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
}
