using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public List<GameObject> playerPrefabs;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreatePlayer(int id)
    {
        //遍歷playerPrefabs，從playerPrefab上拿取PlayerController，檢查PlayerController的playerInputData.id是否等於id
        foreach (var playerPrefab in playerPrefabs)
        {
            PlayerController playerController = playerPrefab.GetComponent<PlayerController>();
            if (playerController.playerInputData.id == id)
            {
                //生成玩家
                GameObject go = Instantiate(playerPrefab);
                PlayerController playerCo = go.GetComponent<PlayerController>();
                playerCo.Init(GameManager.Instance.gameController);
                playerCo.SetCardDealer(playerController.playerInputData.id == 1 ? GameManager.Instance.cardUIManager.LeftCardDealer : GameManager.Instance.cardUIManager.RightCardDealer);
            }
        }
    }

    public void ClearPlayer()
    {
        //遍歷所有玩家，刪除玩家
        foreach (var player in FindObjectsOfType<PlayerController>())
        {
            Destroy(player.gameObject);
        }
    }
}
