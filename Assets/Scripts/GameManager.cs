using System;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameManager : Singleton<GameManager>
{
    public RoomGenerator roomGenerator;
    public bool _isPaused = false;
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
        PrefabManager.Instance.gameOverUI.SetActive(true);
    }

}
