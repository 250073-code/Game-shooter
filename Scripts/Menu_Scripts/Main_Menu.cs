using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    // Loads the main game scene
    public void LoadScene()
    {
        SceneManager.LoadScene(2);
    }
}
