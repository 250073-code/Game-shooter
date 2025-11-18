using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeEnemyScript : MonoBehaviour
{
    private Player _player; // Reference to the player
    private float _distance; // Distance to the player
    [SerializeField]
    private float _speed = 4.0f; // Enemy movement speed
    [SerializeField]
    private float _distanceBetween = 4.0f; // Distance to start chasing player
    [SerializeField]
    private AudioSource _audioSource; // Explosion sound
    private AudioClip _explosionSound; // Explosion sound clip
    [SerializeField]
    private GameObject _explosionPrefab; // Explosion effect prefab

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>(); // Find player reference

        if (_player == null)
        {
            Debug.LogError("here is null check"); // Player not found
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Only move if player exists
        if (_player != null)
        {
            KamikazeEnemy();
        }
    }

    // Handles kamikaze movement logic
    public void KamikazeEnemy()
    {
        _distance = Vector3.Distance(transform.position, _player.transform.position);
        Vector3 direction = _player.transform.position - transform.position;

        // Move down if far from player, otherwise chase player quickly
        if (_distance >= _distanceBetween)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else if (_distance < _distanceBetween)
        {
            _speed = 10f;
            transform.position = Vector3.MoveTowards(this.transform.position, _player.transform.position, _speed * Time.deltaTime);
        }

        // Destroy if off screen
        if (transform.position.y < -8.0f)
        {
            Destroy(this.gameObject);
        }
    }

    // Handles collisions with player and lasers
    private void OnTriggerEnter2D(Collider2D other)
    {
        _audioSource = GetComponent<AudioSource>(); // Get audio source

        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage(); // Damage the player
            }

            _speed = 0;
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity); // Play explosion effect
            _audioSource.clip = _explosionSound; // Play explosion sound    
            Destroy(this.gameObject);
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject); // Destroy the laser

            if (_player != null)
            {
                _player.Score(10); // Add score to player
            }

            _speed = 0;
            _audioSource.clip = _explosionSound; // Play explosion sound
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity); // Play explosion effect
            Destroy(GetComponent<Collider2D>()); // Remove collider to prevent further hits
            Destroy(this.gameObject);
        }
    }
}
