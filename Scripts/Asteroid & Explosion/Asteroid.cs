using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 20f; // Asteroid rotation speed
    [SerializeField]
    private GameObject _explosionPrefab; // Explosion effect prefab
    private SpawnManager _spawnManager; // Reference to SpawnManager

    // Start is called before the first frame update
    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>(); // Find SpawnManager
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the asteroid
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
    }
    
    // Handles collision with lasers: spawns explosion, starts enemy spawning, and destroys asteroid
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity); // Play explosion effect
            Destroy(other.gameObject); // Destroy the laser
            _spawnManager.StartSpawning(); // Start enemy spawning
            Destroy(this.gameObject, 0.2f); // Destroy asteroid after short delay
        }
    }
}
