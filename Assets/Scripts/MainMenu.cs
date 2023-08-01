using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayGame()
    {
        GameManager.Instance.GameStart();
        SceneManager.LoadScene("GameScene");
    }

    public void Continue()
    {
        GameManager.Instance.LoadGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
