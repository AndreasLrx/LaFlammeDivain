using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public RoomGenerator roomGenerator;
    private bool _isPaused = false;
    public bool isPaused { get { return _isPaused; } }

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
}
