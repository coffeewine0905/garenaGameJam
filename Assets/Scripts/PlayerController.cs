using System;
using System.Collections;
using System.Collections.Generic;
using IGS_GAME_EX;
using UnityEngine;
public enum GameState
{
    Start,
    ChooseSpice,
    CardAction,
    GetPizza,
    ShowPizza,
    End
}
public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public GameController gameController;
    public PlayerInputData playerInputData;
    public Transform cardRoot;
    public GameObject cardPrefab;
    public List<GameObject> hpList = new List<GameObject>();
    public SpineAnimationCtrl spineAnimationCtrl;
    private Player player;
    private List<CardController> cardControllers = new List<CardController>();
    private int currentCardIndex = 0;
    private bool myTurn = false;
    private int maxHealth = 0;
    private bool hasGetPizza = false;
    private float ShowPizzaDelay = 0;
    private float ShowHappyDelay = 0;
    private float ShowPepperyDelay = 0;

    public void Init(GameController gameController)
    {
        this.gameController = gameController;
        player = new Player
        {
            ID = playerInputData.id,
            RefreshCardAction = RefreshCard
        };
        maxHealth = player.Health;
        gameController.RegisterPlayer(player);
        gameController.ShowCardAction += ShowCards;
        gameController.TurnStartAction += TurnStart;
        gameController.CardStartAction += CardStart;
        gameController.ReStartAction += Reset;

        int nervousIndex = playerInputData.Animations.FindIndex(x => x.name == "nervous");
        int eatPizzaIndex = playerInputData.Animations.FindIndex(x => x.name == "eat pizza");
        int happyIndex = playerInputData.Animations.FindIndex(x => x.name == "happy");
        int pepperyIndex = playerInputData.Animations.FindIndex(x => x.name == "peppery");
        ShowPizzaDelay += playerInputData.Animations[nervousIndex].Animation.Duration;
        ShowPizzaDelay += playerInputData.Animations[eatPizzaIndex].Animation.Duration;
        ShowHappyDelay += playerInputData.Animations[happyIndex].Animation.Duration;
        ShowPepperyDelay += playerInputData.Animations[pepperyIndex].Animation.Duration;
    }

    private void CardStart(int index)
    {
        if (index == playerInputData.id)
        {
            Debug.Log("Player id: " + player.ID + " CardAction");
            ChooseCard(currentCardIndex);
        }
    }

    public void TurnStart(int index)
    {
        if (index == playerInputData.id)
        {
            myTurn = true;
        }
    }

    void Update()
    {
        if (myTurn)
        {
            switch (gameController.CurrentGameState)
            {
                case GameState.Start:
                    break;
                case GameState.ChooseSpice:
                    SpiceAction();
                    break;
                case GameState.CardAction:
                    CardAction();
                    break;
                case GameState.GetPizza:
                    if (!hasGetPizza)
                    {
                        hasGetPizza = true;
                        GetPizzaAction();
                    }
                    break;
                case GameState.ShowPizza:
                    break;
            }

        }
    }

    private void GetPizzaAction()
    {
        PizzaData pizzaData = gameController.GetPizzaData();
        StartCoroutine(ShowPizza(pizzaData));
        gameController.StartShowPizzaPhase();
    }
    IEnumerator ShowPizza(PizzaData pizzaData)
    {
        GameManager.Instance.uiManager.ShowLog("Show Your Pizza!!");
        spineAnimationCtrl.GetSpineAnime.state.SetAnimation(0, "nervous", false);
        spineAnimationCtrl.AddSpineAnima("eat pizza", true);
        yield return new WaitForSeconds(ShowPizzaDelay);
        float delay = 0;
        if (pizzaData.IsSpicy)
        {
            Debug.Log("Pizza is spicy");
            spineAnimationCtrl.GetSpineAnime.state.SetAnimation(0, "peppery", false);
            spineAnimationCtrl.AddSpineAnima("standby", true);
            GameManager.Instance.uiManager.ShowLog("Pizza is spicy~~~");
            player.Health--;
            for (int i = 0; i < hpList.Count; i++)
            {
                if (i < player.Health)
                {
                    hpList[i].SetActive(true);
                }
                else
                {
                    hpList[i].SetActive(false);
                }
            }
            delay = ShowPepperyDelay;
        }
        else
        {
            Debug.Log("Pizza is not spicy");
            spineAnimationCtrl.GetSpineAnime.state.SetAnimation(0, "happy", false);
            spineAnimationCtrl.AddSpineAnima("standby", true);
            GameManager.Instance.uiManager.ShowLog("SAFE!!");
            delay = ShowHappyDelay;
        }
        yield return new WaitForSeconds(delay);
        myTurn = false;
        hasGetPizza = false;
        gameController.EndTurn();
    }

    private void SpiceAction()
    {
        if (Input.GetKeyDown(playerInputData.MoveUp))
        {
            IncreaseSpice();
        }
        if (Input.GetKeyDown(playerInputData.MoveDown))
        {
            DecreaseSpice();
        }
        if (Input.GetKeyDown(playerInputData.Confirm))
        {
            ConfirmSpice();
        }
    }
    private void CardAction()
    {
        if (Input.GetKeyDown(playerInputData.MoveUp))
        {
            currentCardIndex++;
            ChooseCard(currentCardIndex);
        }
        if (Input.GetKeyDown(playerInputData.MoveDown))
        {
            currentCardIndex--;
            ChooseCard(currentCardIndex);
        }
        if (Input.GetKeyDown(playerInputData.Confirm))
        {
            UseCard();
        }
    }

    private void ChooseCard(int index)
    {
        //檢查index是否超出範圍
        if (index < 0)
        {
            currentCardIndex = 0;
        }
        else if (index >= player.Hand.Count)
        {
            currentCardIndex = player.Hand.Count - 1;
        }
        //遍歷所有卡片，將選中的卡片放大
        for (int i = 0; i < cardControllers.Count; i++)
        {
            if (i == currentCardIndex)
            {
                cardControllers[i].transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }
            else
            {
                cardControllers[i].transform.localScale = Vector3.one;
            }
        }
    }
    private void UseCard()
    {
        //使用卡片
        cardControllers[currentCardIndex].Use();
        //刪除使用過的卡片
        player.Hand.RemoveAt(currentCardIndex);
        //刪除卡片UI
        Destroy(cardControllers[currentCardIndex].gameObject);
        cardControllers.RemoveAt(currentCardIndex);
        //對玩家的卡片進行排序
        SortCards();
        gameController.StartGetPizzaPhase();
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

    public void ClearCards()
    {
        for (int i = 0; i < cardControllers.Count; i++)
        {
            Destroy(cardControllers[i].gameObject);
        }
        cardControllers.Clear();
    }

    public void SortCards()
    {
        for (int i = 0; i < cardControllers.Count; i++)
        {
            cardControllers[i].transform.localPosition = new Vector3(0, i + i * 0.2f, 0);
        }
    }

    public void RefreshCard()
    {
        if (player.Hand.Count != cardControllers.Count && player.Hand.Count > 0)
        {
            ClearCards();
            ShowCards();
        }
    }

    public void IncreaseSpice()
    {
        gameController.IncreaseSpice();
    }

    public void DecreaseSpice()
    {
        gameController.DecreaseSpice();
    }

    public void ConfirmSpice()
    {
        Debug.Log("Player id: " + player.ID + " ConfirmSpice: " + gameController.currentSpice);
        gameController.ConfirmSpice();
    }

    public void Reset()
    {
        myTurn = false;
        hasGetPizza = false;
        currentCardIndex = 0;
        player.Health = maxHealth;
        ClearCards();
    }
}
