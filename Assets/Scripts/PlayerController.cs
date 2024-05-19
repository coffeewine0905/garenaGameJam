using System;
using System.Collections;
using System.Collections.Generic;
using IGS_GAME_EX;
using Unity.VisualScripting;
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
    public bool sortX = true;
    private Player player;
    private List<CardController> cardControllers = new List<CardController>();
    private bool myTurn = false;
    private int maxHealth = 0;
    private bool hasGetPizza = false;
    private float ShowPizzaDelay = 0;
    private float ShowHappyDelay = 0;
    private float ShowPepperyDelay = 0;
    private CardDealer cardDealer;

    public void Init(GameController gameController)
    {
        this.gameController = gameController;
        player = new Player
        {
            ID = playerInputData.id,
            RefreshCardAction = RefreshCard,
            DrawPizzaCount = playerInputData.DrawPizzaCount
        };
        maxHealth = player.Health;
        gameController.RegisterPlayer(player);
        gameController.ShowCardAction += ShowCards;
        gameController.TurnStartAction += TurnStart;
        gameController.TurnEndAction += (id) =>
        {
            if (id == player.ID)
            {
                myTurn = false;
            }
        };
        gameController.CardStartAction += CardStart;
        gameController.AddHpAction += AddHp;
        gameController.HappyAction += Happy;
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

    private void Happy(int id)
    {
        if (id == player.ID)
        {
            spineAnimationCtrl.GetSpineAnime.state.SetAnimation(0, "happy", false);
            spineAnimationCtrl.AddSpineAnima("standby", true);
        }
    }

    private void AddHp(int id)
    {
        if (id == player.ID)
        {
            spineAnimationCtrl.GetSpineAnime.state.SetAnimation(0, "drink milk", false);
            spineAnimationCtrl.AddSpineAnima("standby", true);
            player.Health++;
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
        }
    }

    private void CardStart(int index)
    {
        if (index == playerInputData.id)
        {
            Debug.Log("Player id: " + player.ID + " CardAction");
            cardDealer.FocusCard();
        }
    }

    public void SetCardDealer(CardDealer cardDealer)
    {
        this.cardDealer = cardDealer;
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
        player.DrawPizzaCount--;
        StartCoroutine(ShowPizza(pizzaData));
        gameController.StartShowPizzaPhase();
    }
    IEnumerator ShowPizza(PizzaData pizzaData)
    {
        GameManager.Instance.uiManager.ShowLog("Show Your Pizza!!");
        yield return new WaitForSeconds(1.6f);
        spineAnimationCtrl.GetSpineAnime.state.SetAnimation(0, "nervous", false);
        spineAnimationCtrl.AddSpineAnima("eat pizza", false);
        spineAnimationCtrl.AddSpineAnima("standby", true);
        yield return new WaitForSeconds(ShowPizzaDelay);
        float delay = 0;
        if (pizzaData.IsSpicy && player.CanRedrawPizza)
        {
            player.CanRedrawPizza = false;
            GameManager.Instance.uiManager.ShowLog("You got spicy pizza but.....");
            yield return new WaitForSeconds(1.6f);
            GameManager.Instance.uiManager.ShowLog("Lucky! You can redraw pizza!!");
            yield return new WaitForSeconds(1.6f);
            GetPizzaAction();
            //中斷這個協程
            yield break;
        }
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

        if (player.DrawPizzaCount > 0)
        {
            GetPizzaAction();
        }
        else
        {
            myTurn = false;
            hasGetPizza = false;
            player.CanRedrawPizza = false;
            player.DrawPizzaCount = playerInputData.DrawPizzaCount;
            gameController.EndTurn();
        }
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
        if (Input.GetKeyDown(playerInputData.MoveRight))
        {
            cardDealer.ChooseRightCard();
        }
        if (Input.GetKeyDown(playerInputData.MoveLeft))
        {
            cardDealer.ChooseLeftCard();
        }
        if (Input.GetKeyDown(playerInputData.Confirm))
        {
            UseCard();
        }
    }
    private void UseCard()
    {
        float delay = 0;
        delay = gameController.GetCardDelay(player.Hand[cardDealer.GetCurrentCardIndex()].ID);
        //使用卡片
        cardDealer.UseCard(player.Hand[cardDealer.GetCurrentCardIndex()].ID);
        // cardControllers[currentCardIndex].Use();
        // //刪除使用過的卡片
        // player.Hand.RemoveAt(currentCardIndex);
        // //刪除卡片UI
        // Destroy(cardControllers[currentCardIndex].gameObject);
        // cardControllers.RemoveAt(currentCardIndex);
        // //對玩家的卡片進行排序
        // SortCards();
        //如果卡片使用完畢，則進入下一階段
        StartCoroutine(useCardCoroutine(delay));
    }
    IEnumerator useCardCoroutine(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        gameController.StartGetPizzaPhase();
    }
    public void ShowCards()
    {
        Debug.Log("Show Cards player.Hand.Count: " + player.Hand.Count);
        cardDealer.ShowCard(player.Hand, (cardId) =>
            {
                gameController.UseCard(cardId, player.ID);
            });
        // foreach (var card in player.Hand)
        // {
        //     GameObject cardGo = Instantiate(cardPrefab, cardRoot);
        //     CardController cardController = cardGo.GetComponent<CardController>();
        //     cardController.OnUseAction += (cardId) =>
        //     {
        //         gameController.UseCard(cardId, player.ID);
        //     };
        //     cardController.Init(card, sortX);
        //     cardControllers.Add(cardController);
        // }
        //對玩家的卡片進行排序
        // SortCards();
    }

    public void ClearCards()
    {
        // for (int i = 0; i < cardControllers.Count; i++)
        // {
        //     Destroy(cardControllers[i].gameObject);
        // }
        // cardControllers.Clear();
        cardDealer.ClearCard();
    }

    // public void SortCards()
    // {
    //     for (int i = 0; i < cardControllers.Count; i++)
    //     {
    //         //建立一個變數決定正負
    //         float temp = sortX ? 1 : -1;
    //         cardControllers[i].transform.localPosition = new Vector3(i * 0.2f * temp, 0, 0);
    //     }
    // }

    public void RefreshCard()
    {
        cardDealer.RefreshCard(player.Hand, (cardId) =>
        {
            gameController.UseCard(cardId, player.ID);
        });
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
        cardDealer.Reset();
        player.Health = maxHealth;
        ClearCards();
    }

    private void OnDestroy()
    {
        gameController.ShowCardAction -= ShowCards;
        gameController.TurnStartAction -= TurnStart;
        gameController.CardStartAction -= CardStart;
        gameController.ReStartAction -= Reset;
    }
}
