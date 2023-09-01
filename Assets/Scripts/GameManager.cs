using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : Singleton<GameManager>
{
    public RoomGenerator roomGenerator;
    private bool _isPaused = false;
    public bool isPaused { get { return _isPaused; } }
    public GameObject gameOverUI;

    protected override void Awake()
    {
        base.Awake();
        roomGenerator = GetComponent<RoomGenerator>();
    }

    void Start()
    {
        Invoke("InitGame", 0.1f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            PlayerController.Instance.AddWisp(Instantiate(PrefabManager.GetRandomWisp(), PlayerController.Instance.transform.position, Quaternion.identity, null).GetComponent<Wisp>());
        if (Input.GetKeyDown(KeyCode.M))
            roomGenerator.Regenerate(); 
    }

    void InitGame()
    {
        roomGenerator.GenerateRoom();
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
        
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        _isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        _isPaused = false;
    }


    public void Restart(){
         roomGenerator.Regenerate();
         gameOverUI.SetActive(false);
         PlayerController.instance.setIsDead(false);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        Destroy();
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
