using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls the boss enemy's behavior, firing, and damage handling.
public class BOSS : MonoBehaviour
{
    [SerializeField]
    private int _bossLives = 10; // Number of hits boss can take
    [SerializeField]
    private GameObject _rocket; // Rocket prefab to fire
    [SerializeField]
    private GameObject _bossExplosionPrefab; // Explosion effect prefab
    private Animator _bossAnim; // Reference to boss animator
    private Player _player; // Reference to player
    private UIManager _uiManager; // Reference to UIManager
    private float _fireRate = 3.0f; // Time between rocket shots
    private float _canFire = -1f; // Next time boss can fire
    private bool _canAttack = false; // Is the boss currently attacking?

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0f, 12.0f, 0f); // Set initial position
        _bossAnim = GetComponent<Animator>(); // Get animator component
        _player = GameObject.Find("Player").GetComponent<Player>(); // Find player
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>(); // Find UIManager
        _uiManager.BossHealth(_bossLives); // Update boss health UI

        if (_player == null)
        {
            Debug.LogError("here smth wrong"); // Player not found
        }

        _bossAnim.SetTrigger("BossEntry");
        StartCoroutine(BossRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (_canAttack == true)
        {
            CanAttack();
        }
    }

    // Handles boss attacking by firing rockets
    private void CanAttack()
    {
        _canAttack = true;
        // Fire rocket at set intervals
        if (Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;
            Instantiate(_rocket, transform.position + new Vector3(0f, -2f, 1f), Quaternion.Euler(-180, 0, 0));
        }
    }

    // Handles collision with lasers, boss damage, and death
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject); // Destroy the laser
            _bossLives--; // Decrease boss lives
            _uiManager.BossHealth(_bossLives); // Update boss health UI

            if (_bossLives == 0)
            {
                Instantiate(_bossExplosionPrefab, transform.position, Quaternion.identity); // Play explosion
                Destroy(this.gameObject); // Destroy boss
                _uiManager.VictoryScene(); // Trigger victory UI
            }
        }

        if (other.tag == "Player")
        {
            Player _player = other.transform.GetComponent<Player>();

            if (_player != null)
            {
                _player.Damage(); // Damage the player
            }
        }
    }

    // Coroutine to manage boss behavior sequence
    IEnumerator BossRoutine()
    {
        yield return new WaitForSeconds(3.1f);

        while (this.gameObject != null)
        {
            _canAttack = true;
            _bossAnim.SetTrigger("BossAnim");
            yield return new WaitForSeconds(4.1f); // Wait for animation to play
            
            _canAttack = false; // Not attacking during attack animation
            _bossAnim.SetTrigger("BossAttack");
            yield return new WaitForSeconds(5.0f); // Wait for animation to play
        }
    }
}
