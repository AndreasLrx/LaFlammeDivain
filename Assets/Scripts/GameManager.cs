using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public BoardManager boardScript;

    protected override void Awake()
    {
        base.Awake();
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        boardScript.SetupScene();
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
    }
}
