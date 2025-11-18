using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool _gameOver; // Is the game over?

    // Update is called once per frame
    private void Update()
    {
        // Restart the game if R is pressed and game is over
        if (Input.GetKeyDown(KeyCode.R) && _gameOver == true)
        {
            SceneManager.LoadScene(1); // Reload current scene
        }

        // Quit the game if Escape is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    // Called when the game is over
    public void GameOver()
    {
        _gameOver = true;
    }

    // Called when the player wins
    public void Victory()
    {
        _gameOver = true;
    }
}
