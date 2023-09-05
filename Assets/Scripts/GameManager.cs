using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private bool _isPaused = false;
    public bool isPaused { get { return _isPaused; } }
    public FloorGenerator floorGenerator;

    protected override void Awake()
    {
        base.Awake();
        floorGenerator = GetComponent<FloorGenerator>();
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
            floorGenerator.RegenerateFloor();

    }

    void InitGame()
    {
        floorGenerator.GenerateFloor();
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
    }

    public void ChangeRoom(Room newRoom, Vector2? position = null)
    {
        floorGenerator.roomGenerator.PlacePlayer(newRoom, position);
        floorGenerator.roomGenerator.PlaceEnemies(newRoom);
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
