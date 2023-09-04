using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{ 

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public void MainMenu()
    {
        Time.timeScale = 1f;
        GameManager.Destroy();
        PlayerController.Destroy();
        GameObject DontdestroyOnLoadDestroyer = new GameObject("DontdestroyOnLoadDestroyer");
        DontDestroyOnLoad(DontdestroyOnLoadDestroyer);
        foreach (GameObject root in DontdestroyOnLoadDestroyer.scene.GetRootGameObjects())
        Destroy(root);

        Destroy(FindObjectOfType<Canvas>().gameObject);
        Destroy(FindObjectOfType<Player>().gameObject);
        Destroy(FindObjectOfType<AICompanion>().gameObject);
        SceneManager.LoadScene("Menu");
    }


}
