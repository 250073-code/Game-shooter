using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f; // Rocket movement speed

    // Update is called once per frame
    void Update()
    {
        // Move rocket up
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        // Destroy rocket if it goes off screen
        if (transform.position.y < -8f)
        {
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
