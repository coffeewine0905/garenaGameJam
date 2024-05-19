using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public CardData cardData;
    private CardView cardView;
    public Action<int> OnUseAction;
    // Start is called before the first frame update
    private void Awake()
    {
        cardView = GetComponentInChildren<CardView>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(CardData cardData, bool sortX)
    {
        this.cardData = cardData;
        cardView.Init(cardData, sortX);
    }

    public void Use()
    {
        if (cardData == null)
        {
            return;
        }
        OnUseAction?.Invoke(cardData.ID);
        cardView.Reset();
        cardView.OnDeselect();
    }

    public void OnSelect()
    {
        cardView.OnSelect();
    }

    public void OnDeselect()
    {
        cardView.OnDeselect();
    }

    private void OnDestroy()
    {
        OnUseAction = null;
    }

    public bool isEmpty()
    {
        return cardData == null;
    }
}
