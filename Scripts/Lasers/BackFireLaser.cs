using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackFireLaser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f; // Backfire laser movement speed

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y > 8f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    // Handles collision with player and applies damage
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
        }
    } 
}
