using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles powerup behavior, movement, and player interaction.
public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f; // Powerup falling speed

    [SerializeField] // 0 = triple shot, 1 = speed, 2 = shields, 3 = ammo, 4 = health, 5 = negative, 6 = rare
    private int powerupID;
    [SerializeField]
    private AudioClip _clip; // Sound to play on pickup
    private bool _moveCloser; // Should the powerup move towards the player?
    private Player _player; // Reference to the player
    [SerializeField]
    private GameObject _laser; // Reference to laser GameObject

    // Called on spawn, finds the player reference
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move towards player if magnet effect is active
        if (_moveCloser == true && _player != null)
        {
            if (Vector3.Distance(transform.position, _player.transform.position) > 0f)
            {
                transform.position += (Vector3)(_speed * Time.deltaTime * (_player.transform.position - transform.position).normalized);
            }
        }

        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        // Destroy if off screen
        if (transform.position.y < -5f)
        {
            Destroy(this.gameObject);
        }
    }

    // Handles collision with player and applies the correct powerup effect
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_clip, transform.position); // Play pickup sound

            if (player != null)
            {
                switch (powerupID)
                {
                    case 0:
                        player.TripleShotActive(); // Triple shot powerup
                        break;
                    case 1:
                        player.SpeedBoostActive(); // Speed boost powerup
                        break;
                    case 2:
                        player.ShieldActive(); // Shield powerup
                        break;
                    case 3:
                        player.AmmoPowerupActive(); // Ammo refill
                        break;
                    case 4:
                        player.HealthActive(); // Health restore
                        break;
                    case 6:
                        player.RarePowerup(); // Rare (homing laser) powerup
                        break;
                    case 5:
                        player.NegativePowerup(); // Negative (bad) powerup
                        break;
                    default:
                        Debug.Log("nothing important");
                        break;
                }
            }
            Destroy(this.gameObject); // Remove powerup after pickup
        }

        if (other.tag == "Enemy Laser")
        {
            Destroy(other.gameObject); // Destroy the laser
            Destroy(this.gameObject); // Destroy powerup if hit by enemy laser
        }
    }

    // Subscribe to events for magnet effect
    public void OnEnable()
    {
        PickupPowerup.movePowerupsTowardsPlayer += MoveCloserToPlayer;
        PickupPowerup.dontMoveTowardsPlayer += DontMoveCloserToPlayer;
    }

    // Unsubscribe from events
    public void OnDisable()
    {
        PickupPowerup.movePowerupsTowardsPlayer -= MoveCloserToPlayer;
        PickupPowerup.dontMoveTowardsPlayer -= DontMoveCloserToPlayer;
    }
    
    // Enable moving towards player
    private void MoveCloserToPlayer()
    {
        _moveCloser = true;
    }

    // Disable moving towards player
    private void DontMoveCloserToPlayer()
    {
        _moveCloser = false;
    }
}