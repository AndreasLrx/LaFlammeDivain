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
        Time.timeScale = 1f;
        GameManager.Destroy();
        PlayerController.Destroy();
        var DontdestroyOnLoadDestroyer = new GameObject("DontdestroyOnLoadDestroyer");
        DontDestroyOnLoad(DontdestroyOnLoadDestroyer);
        foreach(var root in DontdestroyOnLoadDestroyer.scene.GetRootGameObjects())
            Destroy(root);
        
        Destroy(FindObjectOfType<Canvas>().gameObject);
        Destroy(FindObjectOfType<Player>().gameObject);        
        Destroy(FindObjectOfType<RoomGenerator>().gameObject);
        Destroy(FindObjectOfType<AICompanion>().gameObject);
        Destroy(GameObject.Find("PoisonWisp"));
        SceneManager.LoadScene("Menu");
    }

    public void PauseButton()
    {
        GameManager.Instance.Pause();
    }
}
