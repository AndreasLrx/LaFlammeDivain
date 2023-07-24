using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        GameManager.Instance.Resume();
    }
    public void Options()
    {
        Debug.Log("Options");
    }
    public void Exit()
    {
        SceneManager.LoadScene("Menu");
    }
    public void PauseButton()
    {
        GameManager.Instance.Pause();
    }
}
