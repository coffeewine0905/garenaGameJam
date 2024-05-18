using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public GameController gameController;
    public PlayerInputData playerInputData;
    public Transform cardRoot;
    public GameObject cardPrefab;
    private int currentSpice = 0;
    private Player player;
    private List<CardController> cardControllers = new List<CardController>();

    public void Init(GameController gameController)
    {
        this.gameController = gameController;
        player = new Player();
        gameController.RegisterPlayer(player);
        gameController.ShowCardAction += ShowCards;
    }

    public void ShowCards()
    {
        Debug.Log("Show Cards player.Hand.Count: " + player.Hand.Count);
        foreach (var card in player.Hand)
        {
            GameObject cardGo = Instantiate(cardPrefab, cardRoot);
            CardController cardController = cardGo.GetComponent<CardController>();
            cardController.Init(card);
            cardControllers.Add(cardController);
        }
        //對玩家的卡片進行排序
        SortCards();
    }

    public void SortCards()
    {
        for (int i = 0; i < cardControllers.Count; i++)
        {
            cardControllers[i].transform.localPosition = new Vector3(0, i + i * 0.2f, 0);
        }
    }

    public void IncreaseSpice()
    {
        currentSpice++;
    }

    public void DecreaseSpice()
    {
        if (currentSpice > 0)
        {
            currentSpice--;
        }
    }

    public void ConfirmSpice()
    {
        // 隨機選擇一片披薩並設置為辣
        int randomIndex = Random.Range(0, gameController.PizzaArray.Count);
        gameController.PizzaArray[randomIndex].IsSpicy = true;

        // 為所有玩家抽取對應數量的卡片
        foreach (var player in gameController.Players)
        {
            player.Hand.AddRange(gameController.DrawCards(currentSpice));
        }

        // 進入使用卡片環節
        gameController.StartCardUsagePhase();
    }
}
