using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public CardData cardData;
    private CardView cardView;
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
        Debug.Log("Use Card: " + cardData.ID);
    }

    public void OnSelect()
    {
        cardView.OnSelect();
    }

    public void OnDeselect()
    {
        cardView.OnDeselect();
    }
}
