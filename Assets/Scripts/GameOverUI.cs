using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{

    public void Retry()
    {
        // GameManager.roomGenerator.Regenerate();
        // SceneManagement.LoadScene("GameScene");

        Debug.Log("Test");
    }

    public void Quit()
    {
        SceneManager.LoadScene("Menu");
    }
}