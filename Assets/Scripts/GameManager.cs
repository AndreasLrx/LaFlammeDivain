using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public RoomGenerator roomGenerator;
    private bool _isPaused = false;
    public bool isPaused { get { return _isPaused; } }
    public bool _isGameLaunched = false;
    public bool isGameLaunched { get { return _isGameLaunched; } }
    public PlayerSave playerSave;

    protected override void Awake()
    {
        base.Awake();
        roomGenerator = GetComponent<RoomGenerator>();
        playerSave = GetComponent<PlayerSave>();
    }

    // void Start()
    // {
    //     if(_isGameLaunched)
    //         Invoke("InitGame", 0.1f);
    // }

    void Update()
    {
        if(!_isGameLaunched)
            return;
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
        Debug.Log("Game Over");
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

    public void Save()
    {
        playerSave.Save();
    }

    public void GameStart()
    {
        _isGameLaunched = true;
        Invoke("InitGame", 0.1f);
    }
    
    public void Load()
    {
        playerSave.Load();
    }

    public void LoadGame()
    {
        _isGameLaunched = true;
        SceneManager.LoadScene("GameScene");
        Invoke("InitGame", 0.1f);
        Invoke("Load", 0.1f);
    }
}
