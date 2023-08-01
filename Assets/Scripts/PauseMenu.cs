using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


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
        SaveGame();
        Destroy();
        SceneManager.LoadScene("Menu");
    }

    private void SaveGame()
    {
        GameManager.Instance.Save();
    }

    private void Destroy()
    {
        Time.timeScale = 1f;
        GameManager.Instance._isGameLaunched = false;
        PlayerController.Destroy();
        GameObject DontdestroyOnLoadDestroyer = new GameObject("DontdestroyOnLoadDestroyer");
        DontDestroyOnLoad(DontdestroyOnLoadDestroyer);
        foreach (GameObject root in DontdestroyOnLoadDestroyer.scene.GetRootGameObjects())
            Destroy(root);

        Destroy(FindObjectOfType<Canvas>().gameObject);
        Destroy(FindObjectOfType<Player>().gameObject);
        Destroy(FindObjectOfType<AICompanion>().gameObject);
    }

    public void PauseButton()
    {
        GameManager.Instance.Pause();
    }
}
