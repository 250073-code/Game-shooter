using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrangeEnemy : MonoBehaviour
{
    [SerializeField]
    private Transform[] _waypoints; // Path waypoints for movement
    [SerializeField]
    private int _wayPointIndex = 0; // Current waypoint index
    [SerializeField]
    private GameObject _enemyLaserPrefab; // Prefab for enemy's laser
    [SerializeField]
    private GameObject _enemyShieldObject; // Shield visual object
    private float _canFire; // Next time enemy can fire
    [SerializeField]
    private float _fireRate = 0.5f; // Time between shots
    [SerializeField]
    private float _speed = 2.0f; // Movement speed
    private Player _player; // Reference to player
    [SerializeField]
    private AudioSource _audioSource; // Audio source for sounds
    private AudioClip _explosionSound; // Explosion sound clip
    [SerializeField]
    private GameObject _explosionPrefab; // Explosion effect prefab
    private bool _shieldActive = true; // Is shield active?

    // Start is called before the first frame update
    void Start()
    {
        transform.position = _waypoints[_wayPointIndex].transform.position; // Set initial position
        _speed = 2.0f;
        _player = GameObject.Find("Player").GetComponent<Player>(); // Find player
    }

    // Update is called once per frame
    void Update()
    {
        // Only act if player exists
        if (_player != null)
        {
            StrangeEnemyMovement(); // Move along waypoints

            if (Time.time > _canFire)
            {
                _fireRate = Random.Range(2.0f, 5.0f); // Randomize fire rate
                _canFire = Time.time + _fireRate;
                StrangeEnemyFire(); // Fire at intervals
            }
        }
    }

    // Handles movement along waypoints and destroys if out of bounds
    public void StrangeEnemyMovement()
    {
        if (_wayPointIndex <= _waypoints.Length - 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, _waypoints[_wayPointIndex].transform.position, _speed * Time.deltaTime);
        }

        if (transform.position == _waypoints[_wayPointIndex].transform.position)
        {
            _wayPointIndex += 1;
            if (_wayPointIndex == _waypoints.Length)
            {
                _wayPointIndex = 0;
            }
        }

        // Destroy if out of horizontal bounds
        if (transform.position.x >= 11.0f)
        {
            Destroy(this.gameObject);
        }
    }

    // Fires enemy laser(s) at intervals
    public void StrangeEnemyFire()
    {
        GameObject enemyLaser = Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
        EnemyLaser[] lasers = enemyLaser.GetComponentsInChildren<EnemyLaser>();
        
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].MoveDown();
        }
    }

    // Handles collisions with player and lasers, including shield logic
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

            _audioSource.clip = _explosionSound;
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity); // Play explosion effect
            Destroy(this.gameObject);
        }

        if (other.tag == "Laser")
        {
            if (_shieldActive == true)
            {
                if (_enemyShieldObject != null)
                {
                    _shieldActive = false;
                    _enemyShieldObject.SetActive(false); // Disable shield visual
                }
            }
            else
            {
                _audioSource.clip = _explosionSound;
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity); // Play explosion effect
                Destroy(this.gameObject);
                if (_player != null)
                {
                    _player.Score(10); // Add score to player
                }
            }
        }
    }
}
