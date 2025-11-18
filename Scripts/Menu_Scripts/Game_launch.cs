using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_launch : MonoBehaviour
{
    // Loads the main game scene
    public void LoadScene()
    {
        SceneManager.LoadScene(1);
    }
}
