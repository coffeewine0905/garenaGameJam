
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    public UnityEvent OnGameStart;
    public UnityEvent<Player> OnGameEnd;
    public GameObject PizzaPrefab;
    public Transform PizzaParent;
    public List<PizzaData> PizzaArray = new List<PizzaData>();
    public List<CardData> CardDeck = new List<CardData>();
    public List<Player> Players = new List<Player>();
    public System.Action ShowCardAction;
    public List<CardAbilityData> CardAbilityDataList = new List<CardAbilityData>();

    private int currentPlayerIndex = 0;
    private GameObject pizza;
    public Action<int> TurnStartAction;
    public Action<int> TurnEndAction;
    public Action<int> CardStartAction;
    public Action ReStartAction;
    public GameState CurrentGameState { get; set; }
    public int currentSpice = 0;

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
        //隨機對 PizzaArray 中的一個披薩設定為辣
        PizzaArray[UnityEngine.Random.Range(0, PizzaArray.Count)].IsSpicy = true;

    }

    public void ConfirmSpice()
    {
        // 根據currentSpice次數隨機對中披薩設定為辣
        for (int i = 0; i < currentSpice; i++)
        {
            foreach (var pizza in PizzaArray)
            {
                if (pizza.IsSpicy == false)
                {
                    pizza.IsSpicy = true;
                    break;
                }
            }
        }
        StartCoroutine(AddCardPhase());


    }
    IEnumerator AddCardPhase()
    {
        // 為所有玩家抽取對應數量的卡片
        GameManager.Instance.uiManager.ShowLog("Add " + currentSpice + " cards");
        foreach (var player in Players)
        {
            player.Hand.AddRange(DrawCards(currentSpice));
            player.RefreshCardAction?.Invoke();
        }
        currentSpice = 0;
        yield return new WaitForSeconds(1.6f);
        // 進入使用卡片環節
        StartCardUsagePhase();
    }

    void InitializeCardDeck()
    {
        for (int i = 0; i < CardAbilityDataList.Count; i++)
        {
            CardDeck.Add(new CardData { ID = CardAbilityDataList[i].ID });
        }
    }

    //註冊玩家
    public void RegisterPlayer(Player player)
    {
        Players.Add(player);
    }

    public void StartGame()
    {
        OnGameStart?.Invoke();
        foreach (var player in Players)
        {
            player.Hand.AddRange(DrawCards(3));
        }
        ShowCardAction?.Invoke();
        InitializePizza();

        StartCoroutine(StartGameCoroutine());
    }
    IEnumerator StartGameCoroutine()
    {
        //TODO: 前置作業
        CurrentGameState = GameState.Start;
        GameManager.Instance.uiManager.ShowLog("Game Start");
        yield return new WaitForSeconds(1.6f);
        StartPlayerTurn();
    }

    internal List<CardData> DrawCards(int count)
    {
        List<CardData> drawnCards = new List<CardData>();
        for (int i = 0; i < count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, CardDeck.Count);
            drawnCards.Add(CardDeck[randomIndex]);
            // CardDeck.RemoveAt(randomIndex);
        }
        return drawnCards;
    }

    public void StartCardUsagePhase()
    {
        StartCoroutine(StartCardUsagePhaseCoroutine());
    }
    IEnumerator StartCardUsagePhaseCoroutine()
    {
        GameManager.Instance.uiManager.ShowLog("Start Card Usage Phase");
        yield return new WaitForSeconds(1.6f);
        // 進入選擇卡片環節
        CurrentGameState = GameState.CardAction;
        CardStartAction?.Invoke(Players[currentPlayerIndex].ID);
    }

    public void StartGetPizzaPhase()
    {
        // 等待玩家選擇披薩
        // 這裡可以實現玩家選擇披薩的邏輯
        CurrentGameState = GameState.GetPizza;
    }

    public void StartShowPizzaPhase()
    {
        StartCoroutine(StartShowPizzaPhaseCoroutine());
    }
    IEnumerator StartShowPizzaPhaseCoroutine()
    {
        GameManager.Instance.uiManager.ShowLog("Start Show Pizza Phase");
        yield return new WaitForSeconds(1.6f);
        // 進入表演吃披薩環節
        CurrentGameState = GameState.ShowPizza;
    }

    public PizzaData GetPizzaData()
    {
        return PizzaArray[UnityEngine.Random.Range(0, PizzaArray.Count)];
    }


    void StartPlayerTurn()
    {
        // 等待玩家操作
        // 這裡可以實現玩家操作介面的顯示
        int currentPlayerID = Players[currentPlayerIndex].ID;
        Debug.Log("Player " + currentPlayerID + " turn start");
        TurnStartAction?.Invoke(currentPlayerID);


        StartCoroutine(StartPlayerChooseSpiceCoroutine());
    }

    IEnumerator StartPlayerChooseSpiceCoroutine()
    {
        int currentPlayerID = Players[currentPlayerIndex].ID;
        GameManager.Instance.uiManager.ShowLog("Player " + currentPlayerID + " turn start");
        yield return new WaitForSeconds(1.6f);
        GameManager.Instance.uiManager.ShowLog("Player id: " + currentPlayerID + " ChooseSpice");
        yield return new WaitForSeconds(1.6f);
        // 進入選擇辣椒環節
        CurrentGameState = GameState.ChooseSpice;
        Debug.Log("Player id: " + currentPlayerID + " ChooseSpice");
    }

    public void EndTurn()
    {
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
    void EndGame()
    {
        // 判斷遊戲結束邏輯
        Debug.Log("Game Over");
        TurnEndAction?.Invoke(Players[currentPlayerIndex].ID);
        OnGameEnd?.Invoke(Players[currentPlayerIndex]);
    }

    public void RestartGame()
    {
        // 重置遊戲數據
        currentPlayerIndex = 0;
        ReStartAction?.Invoke();
        StartGame();
    }

    public void IncreaseSpice()
    {
        currentSpice++;
        Debug.Log("IncreaseSpice: " + currentSpice);
        GameManager.Instance.uiManager.ShowLog("IncreaseSpice: " + currentSpice);
    }

    public void DecreaseSpice()
    {
        if (currentSpice > 0)
        {
            currentSpice--;
            Debug.Log("DecreaseSpice: " + currentSpice);
            GameManager.Instance.uiManager.ShowLog("DecreaseSpice: " + currentSpice);
        }
    }
    public void UseCard(int cardID, int playerID)
    {
        StartCoroutine(useCardCoroutine(cardID, playerID));
    }
    IEnumerator useCardCoroutine(int cardID, int playerID)
    {
        // 這裡可以實現卡片效果的邏輯
        Debug.Log("Player id: " + playerID + " Use Card: " + cardID);
        //從 CardAbilityDataList 中找到對應ID的卡片效果
        CardAbilityData cardAbilityData = CardAbilityDataList.Find(x => x.ID == cardID);
        GameManager.Instance.uiManager.ShowLog("Player id: " + playerID + " Use Card: " + cardAbilityData.Name);
        yield return new WaitForSeconds(cardAbilityData.showDelay);
    }

    public float GetCardDelay(int cardID)
    {
        CardAbilityData cardAbilityData = CardAbilityDataList.Find(x => x.ID == cardID);
        return cardAbilityData.showDelay;
    }
}
