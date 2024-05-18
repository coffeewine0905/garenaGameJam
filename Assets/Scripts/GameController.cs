
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject PizzaPrefab;
    public Transform PizzaParent;
    public List<PizzaData> PizzaArray = new List<PizzaData>();
    public List<CardData> CardDeck = new List<CardData>();
    public List<Player> Players = new List<Player>();
    public System.Action ShowCardAction;

    private int currentPlayerIndex = 0;
    private GameObject pizza;

    void Start()
    {

        InitializeCardDeck();
    }

    void InitializePizza()
    {
        if (pizza != null)
        {
            Destroy(pizza);
        }
        if (PizzaArray.Count > 0)
        {
            PizzaArray.Clear();
        }
        for (int i = 0; i < 10; i++)
        {
            PizzaArray.Add(new PizzaData());
        }
        //產生一個披薩PizzaPrefab在PizzaParent下
        pizza = Instantiate(PizzaPrefab, PizzaParent);

    }

    void InitializeCardDeck()
    {
        for (int i = 0; i < 11; i++)
        {
            CardDeck.Add(new CardData { ID = i });
        }
    }

    //註冊玩家
    public void RegisterPlayer(Player player)
    {
        Players.Add(player);
    }

    public void StartGame()
    {
        foreach (var player in Players)
        {
            player.Hand.AddRange(DrawCards(3));
        }
        ShowCardAction?.Invoke();
        InitializePizza();

        StartPlayerTurn();
    }

    internal List<CardData> DrawCards(int count)
    {
        List<CardData> drawnCards = new List<CardData>();
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, CardDeck.Count);
            drawnCards.Add(CardDeck[randomIndex]);
            // CardDeck.RemoveAt(randomIndex);
        }
        return drawnCards;
    }

    public void StartCardUsagePhase()
    {
        // 等待玩家選擇並使用卡片
        // 這裡可以實現玩家選擇卡片的邏輯
    }

    public void EndTurn()
    {
        int randomPizzaIndex = Random.Range(0, PizzaArray.Count);
        PizzaData selectedPizza = PizzaArray[randomPizzaIndex];

        if (selectedPizza.IsSpicy)
        {
            Players[currentPlayerIndex].Health -= 1;
        }

        if (Players[currentPlayerIndex].Health <= 0)
        {
            EndGame();
        }
        else
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % Players.Count;
            StartPlayerTurn();
        }
    }

    void StartPlayerTurn()
    {
        // 等待玩家操作
        // 這裡可以實現玩家操作介面的顯示
    }

    void EndGame()
    {
        // 判斷遊戲結束邏輯
        Debug.Log("Game Over");
    }
}
