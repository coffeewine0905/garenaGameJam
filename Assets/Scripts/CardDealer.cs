using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDealer : MonoBehaviour
{
    public Transform CardParent;
    public GameObject CardPrefab;
    public int maxCount = 4;
    private int currentCardIndex = 0;
    private int lastCardIndex = -1;
    private List<CardData> cardDatas = new List<CardData>();
    private List<CardController> cardControllers = new List<CardController>();
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowCard(List<CardData> cardDatas, Action<int> OnUseAction)
    {
        if (cardControllers.Count != 0)
        {
            ClearCard();
        }
        //產生到maxCount數量的卡牌
        for (int i = 0; i < maxCount; i++)
        {
            GameObject card = Instantiate(CardPrefab, CardParent);
            cardControllers.Add(card.GetComponent<CardController>());
        }
        this.cardDatas = cardDatas;
        for (int i = 0; i < this.cardDatas.Count; i++)
        {
            cardControllers[i].Init(this.cardDatas[i], true);
            cardControllers[i].OnUseAction = OnUseAction;

        }
    }

    public int GetCurrentCardIndex()
    {
        return currentCardIndex;
    }

    public void FocusCard()
    {
        cardControllers[currentCardIndex].OnSelect();
        lastCardIndex = currentCardIndex;
    }

    public void ChooseRightCard()
    {
        currentCardIndex++;
        ChooseCard(currentCardIndex);
    }

    public void ChooseLeftCard()
    {
        currentCardIndex--;
        ChooseCard(currentCardIndex);
    }

    public void ChooseCard(int index)
    {
        //檢查index是否超出範圍
        if (index < 0)
        {
            currentCardIndex = 0;
            return;
        }
        else if (index >= cardControllers.Count)
        {
            currentCardIndex = cardControllers.Count - 1;
            return;
        }
        Debug.Log("currentCardIndex: " + currentCardIndex);
        GameManager.Instance.audioManager.PlaySound("ui_menu_button_beep_13");

        if (lastCardIndex != -1 && lastCardIndex != currentCardIndex && lastCardIndex < cardControllers.Count)
        {
            cardControllers[lastCardIndex].transform.localScale = Vector3.one;
            cardControllers[lastCardIndex].OnDeselect();
        }

        cardControllers[currentCardIndex].OnSelect();
        lastCardIndex = currentCardIndex;
    }

    public void UseCard()
    {
        if (currentCardIndex >= cardDatas.Count)
        {
            return;
        }
        cardControllers[currentCardIndex].Use();
        cardDatas.RemoveAt(currentCardIndex);
    }

    public void ClearCard()
    {
        foreach (Transform card in CardParent)
        {
            Destroy(card.gameObject);
        }
    }

    public void RefreshCard(List<CardData> cardDatas, Action<int> OnUseAction)
    {
        for (int i = 0; i < cardControllers.Count; i++)
        {
            cardControllers[i].Init(null, true);
            cardControllers[i].OnUseAction = null;
        }
        Debug.Log("RefreshCard cardDatas count : " + cardDatas.Count);
        this.cardDatas = cardDatas;
        for (int i = 0; i < this.cardDatas.Count; i++)
        {
            cardControllers[i].Init(this.cardDatas[i], true);
            cardControllers[i].OnUseAction = OnUseAction;

        }
    }

    public void Reset()
    {
        currentCardIndex = 0;
        lastCardIndex = -1;
    }
}
