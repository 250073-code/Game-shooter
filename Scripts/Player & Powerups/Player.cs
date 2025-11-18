using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f; // Base movement speed
    private float _speedboost = 2; // Speed multiplier for speed boost
    [SerializeField]
    private float _speedIncrease = 7.0f; // Speed when thruster is active
    private float _currentSpeed; // Current movement speed
    [SerializeField]
    private GameObject _laserPrefab; // Prefab for single laser shot
    [SerializeField]
    private GameObject _tripleShotPrefab; // Prefab for triple shot
    [SerializeField]
    private GameObject _hoomingLaser; // Prefab for homing laser
    [SerializeField]
    private float _fireRate = 0.5f; // Time between shots
    private float _canFire = -1f; // Next time the player can fire
    [SerializeField]
    private int _lives = 3; // Player's starting lives
    private SpawnManager _spawnManager; // Reference to SpawnManager
    private bool _isTripleShotActive = false; // Is triple shot powerup active?
    private bool _speedboostActive = false; // Is speed boost powerup active?
    private bool _shieldpowerup = false; // Is shield powerup active?
    private bool _ammoPowerup = false; // Is ammo powerup active?
    [SerializeField]
    private int _ammo = 15; // Current ammo count
    [SerializeField]
    private GameObject _visualizer; // Shield visualizer object
    [SerializeField]
    private int _score; // Player's score
    private UIManager _uimanager; // Reference to UIManager
    [SerializeField]
    private GameObject _rightEngine, _leftEngine; // Engine damage visuals
    [SerializeField]
    private AudioSource _audioData; // Audio source for player sounds
    [SerializeField]
    private AudioClip _explosionSound; // Sound for player explosion
    [SerializeField]
    private AudioClip _laserSound; // Sound for laser firing
    [SerializeField]
    private float _thrusterBarPercentage; // Thruster energy bar
    private bool _isthrusterBoostActive = false; // Is thruster boost active?
    private bool _thrusterRecover = false; // Is thruster recovering?
    private bool _isthrusterBaseActive = true; // Is base thruster active?
    private bool _isHoomingLaserActive = false; // Is homing laser powerup active?
    private bool _isNegativePowerupActive = false; // Is negative powerup active?
    private SpriteRenderer _shieldSpriteRenderer; // Renderer for shield color
    private int _shieldLife; // Shield hit points
    private Animator _anim; // Camera animator for damage effect
    [SerializeField]
    private Renderer _targetRenderer; // Renderer for damage flash effect
    private Color _damageColor = Color.red; // Color to flash on damage
    public float flashDuration = 0.2f; // Duration of damage flash
    private Color originalColor; // Original color for damage flash

    // Start is called before the first frame update
    void Start()
    {
        // Initialize references, set starting values, and check for missing components
        _thrusterBarPercentage = 100;
        _currentSpeed = _speed;
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uimanager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioData = GetComponent<AudioSource>();
        _shieldSpriteRenderer = _visualizer.GetComponent<SpriteRenderer>();
        _anim = GameObject.Find("Main Camera").GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("here also"); // Animator not found
        }

        if (_spawnManager == null)
        {
            Debug.LogError("idk what to say"); // SpawnManager not found
        }

        if (_uimanager == null)
        {
            Debug.LogError("idk what to say either"); // UIManager not found
        }
        if (_audioData == null)
        {
            Debug.LogError("here is problem"); // AudioSource not found
        }
        else
        {
            _audioData.clip = _laserSound;
        }

        if (_targetRenderer != null)
        {
            originalColor = _targetRenderer.material.color;
        }
        else
        {
            Debug.LogWarning("Target Renderer not assigned in PlayerDamageEffect script.");
        }

        if (_isNegativePowerupActive == true)
        {
            Debug.LogError("Negative powerup is active");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Handle movement and shooting input
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            Firelaser();
        }
    }

    // Handles player movement, screen wrapping, and thruster logic
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * _currentSpeed * Time.deltaTime);

        // Clamp player position vertically and wrap horizontally
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -4f, 0), 0);

        if (transform.position.x > 11f)
        {
            transform.position = new Vector3(-11f, transform.position.y, 0);
        }
        else if (transform.position.x < -11f)
        {
            transform.position = new Vector3(11f, transform.position.y, 0);
        }

        // Handle thruster boost and recovery
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (_thrusterRecover == false)
            {
                ThrusterBoostActive();
                _currentSpeed = _speedIncrease;
            }
            else if (_thrusterRecover == true)
            {
                ThrusterBaseActive();
                _currentSpeed = _speed;
            }
            if (_thrusterBarPercentage <= 0f)
            {
                _thrusterBarPercentage = 0;
                ThrusterBaseActive();
            }
            StopCoroutine(ThrusterRecoverRoutine());
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _currentSpeed = _speed;
            ThrusterBaseActive();
        }
        else if (_thrusterBarPercentage <= 0)
        {
            StartCoroutine(ThrusterRecoverRoutine());
            ThrusterBaseActive();
            if (_thrusterBarPercentage >= 100)
            {
                _thrusterBarPercentage = 100;
                StopCoroutine(ThrusterRecoverRoutine());
            }
        }
    }

    // Handles firing logic for all shot types and ammo management
    void Firelaser()
    {
        _canFire = Time.time + _fireRate;
        if (_isTripleShotActive == true && _ammo > 2)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            _ammo -= 3;
            UpdateAmmo(_ammo);
        }
        else if (_isTripleShotActive == false && _ammo > 0 && _isHoomingLaserActive == false)
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.25f, 0), Quaternion.identity);
            _ammo -= 1;
            UpdateAmmo(_ammo);
        }

        if (_isHoomingLaserActive == true && _ammo > 0)
        {
            Instantiate(_hoomingLaser, transform.position, Quaternion.identity);
            _ammo -= 1;
            UpdateAmmo(_ammo);
        }

        if (_ammo > 0)
        {
            _audioData.Play();
        }
    }

    // Handles player taking damage, shield logic, and death
    public void Damage()
    {
        if (_shieldpowerup == true)
        {
            if (_shieldLife == 3)
            {
                _shieldLife--;
                _shieldSpriteRenderer.color = new Color32(225, 57, 250, 255);
            }
            else if (_shieldLife == 2)
            {
                _shieldLife--;
                _shieldSpriteRenderer.color = Color.red;
            }
            else if (_shieldLife == 1)
            {
                _shieldLife--;
                _shieldpowerup = false;
                _visualizer.SetActive(false);
            }
            return;
        }

        _lives--;

        _anim.SetTrigger("PlayerDamage");

        if (_targetRenderer != null)
        {
            StartCoroutine(FlashColor());
        }

        if (_lives == 2)
        {
            _rightEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _leftEngine.SetActive(true);
        }

        _uimanager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            _audioData.clip = _explosionSound;
            _audioData.Play();
            Destroy(this.gameObject);
        }
    }
    
    // Activates triple shot powerup for a limited time
    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }
    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    // Activates homing laser powerup for a limited time
    public void RarePowerup()
    {
        _isHoomingLaserActive = true;
        StartCoroutine(RarePowerupRoutine());
    }
    IEnumerator RarePowerupRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isHoomingLaserActive = false;
    }

    // Activates speed boost powerup for a limited time
    public void SpeedBoostActive()
    {
        _speedboostActive = true;
        _currentSpeed *= _speedboost;
        StartCoroutine(SpeedBoostRoutine());
    }
    IEnumerator SpeedBoostRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _speedboostActive = false;
        _currentSpeed /= _speedboost;
    }

    // Activates negative powerup (damages player and removes ammo)
    public void NegativePowerup()
    {
        _isNegativePowerupActive = true;
        if (_isNegativePowerupActive == true)
        {
            _speedboostActive = false;
            _speed = 4.0f;
            Damage();
            _ammo = 0;
            UpdateAmmo(_ammo);
        }
    }

    // Activates shield powerup and sets shield color/life
    public void ShieldActive()
    {
        _shieldpowerup = true;
        _visualizer.SetActive(true);
        _shieldSpriteRenderer.color = Color.white;
        _shieldLife = 3;
    }

    // Refills ammo to max
    public void AmmoPowerupActive()
    {
        _ammoPowerup = true;
        _ammo = 15;
        UpdateAmmo(_ammo);
    }

    // Adds points to score and triggers boss spawn at 200 points
    public void Score(int points)
    {
        _score += points;
        _uimanager.UpdateScore(_score);
    }

    // Sets speed to base value
    void ThrusterBaseActive()
    {
        if (_isthrusterBaseActive == true)
        {
            _speed = 4.0f;
        }
    }

    // Activates thruster boost and drains energy
    void ThrusterBoostActive()
    {
        if (_thrusterBarPercentage > 1 || _thrusterBarPercentage < 100)
        {
            _thrusterRecover = false;
            _isthrusterBoostActive = true;
            _thrusterBarPercentage -= 5.0f * 5 * Time.deltaTime;
        }
        _uimanager.UpdateThrusterBoost(_thrusterBarPercentage);
    }

    // Coroutine to recover thruster energy over time
    IEnumerator ThrusterRecoverRoutine()
    {
        while (_thrusterBarPercentage <= 100f)
        {
            yield return new WaitForSeconds(.1f);
            _thrusterRecover = true;
            _thrusterBarPercentage += 300 / 10 * Time.deltaTime;
            _uimanager.UpdateThrusterBoost(_thrusterBarPercentage);
            yield return new WaitForSeconds(.05f);

            if (_thrusterBarPercentage >= 100f)
            {
                _thrusterRecover = false;
                _thrusterBarPercentage = 100;
                _uimanager.UpdateThrusterBoost(_thrusterBarPercentage);
                break;
            }
        }
    }

    // Coroutine to flash player color on damage
    IEnumerator FlashColor()
    {
        _targetRenderer.material.color = _damageColor;
        yield return new WaitForSeconds(flashDuration);
        _targetRenderer.material.color = originalColor;
    }

    // Updates ammo display in UI
    public void UpdateAmmo(int _ammo)
    {
        _uimanager.UpdatePlayerAmmo(_ammo);
    }

    // Heals player by one life and updates engine visuals
    public void HealthActive()
    {
        if (_lives == 2)
        {
            _lives++;
            _rightEngine.SetActive(false);
            _uimanager.UpdateLives(_lives);
        }
        else if (_lives == 1)
        {
            _lives++;
            _leftEngine.SetActive(false);
            _uimanager.UpdateLives(_lives);
        }
    }
}    