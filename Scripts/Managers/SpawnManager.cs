using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _bossSpawnPrefab; // Boss prefab
    private bool _bossSpawned = false; // Has the boss spawned?
    [SerializeField]
    private GameObject _enemyContainer; // Parent for all enemies
    [SerializeField]
    private GameObject _rarePowerup; // Rare powerup prefab
    [SerializeField]
    private GameObject[] powerups; // Array of powerup prefabs
    private Player _player; // Reference to player
    private bool _stopEnemySpawning = false; // Should enemy spawning stop?
    private bool _stopPowerupSpawning = false; // Should powerup spawning stop?
    // Class for wave configuration
    [System.Serializable]
    public class Waves
    {
        public string waveName; // Name of the wave
        public int numberOfWaves; // Number of enemies in this wave
        public GameObject[] typeOfEnemies; // Enemy types in this wave
        public float spawnInterval; // Time between spawns
    }
    [SerializeField]
    private Waves[] _waves; // All waves
    [SerializeField]
    private Waves _currentWave; // Current wave
    [SerializeField]
    private int _currentWaveNumber; // Current wave index
    private float _nextSpawnTime; // Next time to spawn an enemy
    private bool _canSpawn = true; // Can spawn enemies now?
    private UIManager _uiManager; // Reference to UIManager

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>(); // Find player
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>(); // Find UIManager

        if (_uiManager == null)
        {
            Debug.LogError("here smth wrong"); // UIManager not found
        }
    }

    // Starts all spawn coroutines
    public void StartSpawning()
    {
        StartCoroutine(SpawnRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(RarePowerupSpawnRoutine());
    }

    // Advances to the next wave
    public void SpawnNextWave()
    {
        _currentWaveNumber++;
        _canSpawn = true;
        _uiManager.UpdateCurrentWave(_currentWaveNumber + 1); // Update wave UI
    }

    // Update is called once per frame
    void Update()
    {
        _currentWave = _waves[_currentWaveNumber]; // Update current wave
        GameObject[] totalEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        // If all enemies are gone and more waves remain, start next wave
        if (totalEnemies.Length == 0 && !_canSpawn && _currentWaveNumber + 1 != _waves.Length)
        {
            SpawnNextWave();
        }

        // If all waves completed and boss not spawned, spawn boss
        if (_currentWaveNumber + 1 == _waves.Length && !_bossSpawned)
        {    
            _bossSpawned = true;
            if (_bossSpawned == true)
            {
                _stopEnemySpawning = true;
                if (_stopEnemySpawning == true)
                {
                    StartCoroutine(BossSpawnRoutine());
                }
            }
        }
    }

    // Spawns regular powerups at intervals
    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopPowerupSpawning == false)
        {
            Vector3 posToPowerupSpawn = new Vector3(Random.Range(-8, 8), 8, 0);
            int randompowerup = Random.Range(0, 6);
            Instantiate(powerups[randompowerup], posToPowerupSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(4, 8));
        }
    }

    // Spawns rare powerups at intervals
    IEnumerator RarePowerupSpawnRoutine()
    {
        yield return new WaitForSeconds(30.0f);

        while (_stopPowerupSpawning == false)
        {
            Vector3 posToPowerupSpawn = new Vector3(Random.Range(-8, 8), 8, 0);
            Instantiate(_rarePowerup, posToPowerupSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(20, 30));
        }
    }

    // Stops all spawning when player dies
    public void OnPlayerDeath()
    {
        _stopEnemySpawning = true;
        _stopPowerupSpawning = true;
    }

    // Coroutine to spawn the boss after a delay
    IEnumerator BossSpawnRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        if (_bossSpawned == true)
        {
            GameObject boss = Instantiate(_bossSpawnPrefab, new Vector3(0, 12.0f, 0), Quaternion.identity);
            boss.transform.parent = _enemyContainer.transform;
        }
    }

    // Spawns enemies for the current wave
    IEnumerator SpawnRoutine()
    {
        yield return null;

        while (_stopEnemySpawning == false)
        {
            if (_canSpawn && _nextSpawnTime < Time.time)
            {
                GameObject randomEnemy = _currentWave.typeOfEnemies[Random.Range(0, _currentWave.typeOfEnemies.Length)];
                Vector3 posToSpawn = new Vector3(Random.Range(-8, 8), 8, 0);
                GameObject newEnemy = Instantiate(randomEnemy, posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
                _currentWave.numberOfWaves--;
                _nextSpawnTime = Time.time + _currentWave.spawnInterval;

                if (_currentWave.numberOfWaves == 0)
                {
                    _canSpawn = false;
                }
            }
            yield return new WaitForSeconds(3f);
        }
    }
}