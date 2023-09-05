using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    // public Button exitText;

    public UnityEngine.UI.Image background;
    public Button ResumeButton;
    public Button OptionsButton;
    public Button ExitButton;
    public Button pauseButton;
    public TextMeshProUGUI text;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pauseButton = pauseButton.GetComponent<Button>();
        background = background.GetComponent<UnityEngine.UI.Image>();
        ResumeButton = ResumeButton.GetComponent<Button>();
        OptionsButton = OptionsButton.GetComponent<Button>();
        ExitButton = ExitButton.GetComponent<Button>();
        text = text.GetComponent<TextMeshProUGUI>();

        background.gameObject.SetActive(false);
        ResumeButton.gameObject.SetActive(false);
        OptionsButton.gameObject.SetActive(false);
        ExitButton.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.Pause();
            background.gameObject.SetActive(true);
            ResumeButton.gameObject.SetActive(true);
            OptionsButton.gameObject.SetActive(true);
            ExitButton.gameObject.SetActive(true);
            text.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(false);
        }
    }
    public void Resume()
    {
        background.gameObject.SetActive(false);
        ResumeButton.gameObject.SetActive(false);
        OptionsButton.gameObject.SetActive(false);
        ExitButton.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
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
        GameObject DontdestroyOnLoadDestroyer = new GameObject("DontdestroyOnLoadDestroyer");
        DontDestroyOnLoad(DontdestroyOnLoadDestroyer);
        foreach (GameObject root in DontdestroyOnLoadDestroyer.scene.GetRootGameObjects())
            Destroy(root);

        Destroy(FindObjectOfType<Canvas>().gameObject);
        Destroy(FindObjectOfType<Player>().gameObject);
        Destroy(FindObjectOfType<AICompanion>().gameObject);
        SceneManager.LoadScene("Menu");
    }
    public void PauseButton()
    {
        GameManager.Instance.Pause();
    }

}
