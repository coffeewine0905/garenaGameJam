using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public PlayerManager playerManager;
    public GameController gameController;
    public UIManager uiManager;
    public CardUIManager cardUIManager;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateOffLine2Player()
    {
        playerManager.CreatePlayer(1);
        playerManager.CreatePlayer(2);
        gameController.StartGame();
    }

    public void RestartOffLine2Player()
    {
        gameController.ResetGame();
        playerManager.ClearPlayer();
        playerManager.CreatePlayer(1);
        playerManager.CreatePlayer(2);
        gameController.StartGame();
    }
}
