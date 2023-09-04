using UnityEngine;

public class PauseMenu : Menu
{ 

 
    public void Pause()
    {
        Time.timeScale = 0f;
        GameManager.Instance._isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        GameManager.Instance._isPaused = false;
    }

    public void Options()
    {
        Debug.Log("Options");
    }
}
