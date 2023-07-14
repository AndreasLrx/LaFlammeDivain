using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public RoomGenerator roomGenerator;

    protected override void Awake()
    {
        base.Awake();
        roomGenerator = GetComponent<RoomGenerator>();
    }

    void Start()
    {
        InitGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            Player.Instance.AddWisp(Instantiate(PrefabManager.GetRandomWisp(), Player.Instance.transform.position, Quaternion.identity, null).GetComponent<Wisp>());
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
}
