using UnityEngine;

public class GameOverMenu : Menu
{ 


    public void Restart(){
        GameManager.Instance.roomGenerator.Regenerate();
        GameManager.Instance.gameOverUI.SetActive(false);
        PlayerController.Instance.setIsDead(false);
    }

}
