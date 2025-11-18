using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f; // Enemy movement speed
    private Player _player; // Reference to the Player script
    private Animator m_Animator; // Reference to the Animator component
    [SerializeField]
    private AudioSource _audioSource; // Reference to the AudioSource component
    private AudioClip _explosionSound; // Sound played on enemy death
    [SerializeField]
    private GameObject _laserPrefab; // Prefab for the enemy's laser attack
    [SerializeField]
    private GameObject _backFirePrefab; // Prefab for the enemy's back fire effect
    [SerializeField]
    private float _minDistance; // Minimum distance to the player to start attacking
    private bool _backFireActive = true; // Is the back fire effect active
    private float _fireRate = 3.0f; // Time between shots
    private float _canFire = -1; // Next time the enemy can fire

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>(); // Find player reference

        if (_player == null)
        {
            Debug.LogError("here smth wrong"); // Player not found
        }

        m_Animator = GetComponent<Animator>(); // Get animator

        if (m_Animator == null)
        {
            Debug.LogError("here also"); // Animator not found
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement(); // Move the enemy down the screen

        // Handle enemy shooting
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f); // Randomize fire rate
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            EnemyLaser[] lasers = enemyLaser.GetComponentsInChildren<EnemyLaser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].MoveDown(); // Set laser as enemy laser
            }
        }

        if (_player != null)
        {
            BackFireActive(); // Check if back fire effect should be activated
        }
    }

    // Moves the enemy down and destroys it if it goes off screen
    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -8f)
        {
            Destroy(this.gameObject);
        }
    }

    // Activates back fire effect if player is above and within range
    public void BackFireActive()
    {
        float distanceX = Mathf.Abs(_player.transform.position.x - transform.position.x); // Calculate horizontal distance to player

        if (_player.transform.position.y > transform.position.y && distanceX < _minDistance && _backFireActive)
        {
            Instantiate(_backFirePrefab, transform.position + new Vector3(0, 2.2f, 0), Quaternion.identity); // Activate back fire effect
            _backFireActive = false; // Prevent multiple instantiations
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

            m_Animator.SetTrigger("OnEnemyDeath"); // Play death animation
            _backFireActive = false; // Disable back fire effect
            _speed = 0; // Stop moving
            _audioSource.clip = _explosionSound; // Play death sound
            Destroy(GetComponent<Collider2D>()); // Remove collider to prevent further hits
            Destroy(this.gameObject, 2.5f); // Destroy after animation
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject); // Destroy the laser

            if (_player != null)
            {
                _player.Score(10); // Add score to player
            }

            _backFireActive = false; // Disable back fire effect
            m_Animator.SetTrigger("OnEnemyDeath"); // Play death animation
            _speed = 0; // Stop moving
            _audioSource.clip = _explosionSound; // Play death sound
            Destroy(GetComponent<Collider2D>()); // Remove collider to prevent further hits
            Destroy(this.gameObject, 2.5f); // Destroy after animation
        }
    }
    
    // Initiates a dodge maneuver
    public void Dodge()
    {
        float dodgeDirection = Random.Range(0f, 1f); // Randomly choose dodge direction

        if (dodgeDirection < 0.5f)
        {
            StartCoroutine(DodgeLeft()); // Dodge left
        }
        else
        {
            StartCoroutine(DodgeRight()); // Dodge right
        }
    }

    // Coroutine to dodge right
    IEnumerator DodgeRight()
    {
        Vector3 _currentPosition = transform.position;
        Vector3 _targetPosition = new Vector3(_currentPosition.x + 2.0f, _currentPosition.y);
        float _dodge = 0f;

        while (_dodge < 1.0f)
        {
            _dodge += Time.deltaTime * 2.0f; // Adjust speed of dodge here
            transform.position = Vector3.Lerp(_currentPosition, _targetPosition, _dodge);
            yield return null;
        }
    }

    // Coroutine to dodge left
    IEnumerator DodgeLeft()
    {
        Vector3 _currentPosition = transform.position;
        Vector3 _targetPosition = new Vector3(_currentPosition.x - 2.0f, _currentPosition.y);
        float _dodge = 0f;

        while (_dodge < 1.0f)
        {
            _dodge += Time.deltaTime * 2.0f; // Adjust speed of dodge here
            transform.position = Vector3.Lerp(_currentPosition, _targetPosition, _dodge);
            yield return null;
        }
    }
}