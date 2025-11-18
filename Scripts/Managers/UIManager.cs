using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText; // UI text for score
    [SerializeField]
    private Text _ammoText; // UI text for ammo
    [SerializeField]
    private Text _currentWaveText; // UI text for current wave
    [SerializeField]
    private Image _livesImage; // UI image for lives
    [SerializeField]
    private Text _gameOvertext; // UI text for game over
    [SerializeField]
    private Text _victoryText; // UI text for victory
    [SerializeField]
    private Text _restartScene; // UI text for restart prompt
    [SerializeField]
    private Sprite[] _livesSprites; // Sprites for different life counts
    private GameManager _gameManager; // Reference to GameManager
    private SpawnManager _spawnManager; // Reference to SpawnManager
    [SerializeField]
    private Slider _thrusterBar; // UI slider for thruster boost
    [SerializeField]
    TMP_Text _thrusterBarPercentage; // UI text for thruster percentage
    [SerializeField]
    private Slider _bossHealthBar; // UI slider for boss health


    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _ammoText.text = "Ammo: " + 15;
        _currentWaveText.text = "Wave: " + 1;
        _gameOvertext.gameObject.SetActive(false);
        _victoryText.gameObject.SetActive(false);
        _bossHealthBar.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>(); // Find GameManager
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>(); // Find SpawnManager

        if (_gameManager == null)
        {
            Debug.LogError("smth wrong"); // GameManager not found
        }
    }

    // Updates the score UI
    public void UpdateScore(int playerscore)
    {
        _scoreText.text = "Score:" + playerscore.ToString();
    }

    // Updates the ammo UI
    public void UpdatePlayerAmmo(int playerammo)
    {
        _ammoText.text = "Ammo: " + playerammo.ToString();
    }

    // Updates the current wave UI
    public void UpdateCurrentWave(int currentWave)
    {
        _currentWaveText.text = "Wave: " + currentWave.ToString();

        if (currentWave == 3)
        {
            _currentWaveText.text = "Final Wave!";
        }
        else if (currentWave == 4)
        {
            _bossHealthBar.gameObject.SetActive(true);
            _currentWaveText.text = "Boss Fight!";
        }
    }

    // Updates the boss health UI
    public void BossHealth(int currentHealth)
    {
        _bossHealthBar.value = currentHealth;
    }

    // Updates the lives UI and triggers game over if lives reach zero
    public void UpdateLives(int currentLives)
    {
        _livesImage.sprite = _livesSprites[currentLives];
        if (currentLives == 0)
        {
            GameOverScene();
        }
    }

    // Handles game over UI and state
    void GameOverScene()
    {
        _gameManager.GameOver();
        _gameOvertext.gameObject.SetActive(true);
        _restartScene.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    // Coroutine to flicker the game over text
    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOvertext.text = "GAME OVER SYBAU";
            yield return new WaitForSeconds(0.5f);
            _gameOvertext.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Handles victory UI and state
    public void VictoryScene()
    {
        _gameManager.Victory();
        _victoryText.gameObject.SetActive(true);
        _restartScene.gameObject.SetActive(true);
        StartCoroutine(VictoryFlickerRoutine());
    }

    // Coroutine to flicker the victory text
    IEnumerator VictoryFlickerRoutine()
    {
        while (true)
        {
            _victoryText.text = "VICTORY!";
            yield return new WaitForSeconds(0.5f);
            _victoryText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Updates the thruster boost UI bar and percentage
    public void UpdateThrusterBoost(float currentBoostLevel)
    {
        currentBoostLevel = Mathf.Clamp(currentBoostLevel, 0f, 100f);
        _thrusterBar.value = currentBoostLevel;
        _thrusterBarPercentage.text = Mathf.RoundToInt(currentBoostLevel) + "%";
    }
}
