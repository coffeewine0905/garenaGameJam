using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UguiCardView : CardView
{
    public Image Image;
    public GameObject Indicator;
    private RectTransform rectTransform;
    public override void Init(CardData cardData, bool sortX)
    {
        Image.sprite = cardData.Image;
        rectTransform = Image.GetComponent<RectTransform>();
    }

    public override void OnSelect()
    {
        //選擇卡片時，將卡片rectTransform往上移動
        if (rectTransform)
            rectTransform.anchoredPosition += new Vector2(0, OnSelectOffset);
        Indicator.SetActive(true);
    }

    public override void OnDeselect()
    {
        //取消選擇卡片時，將卡片rectTransform回到原點
        if (rectTransform)
            rectTransform.anchoredPosition = new Vector2(0, 0);
        Indicator.SetActive(false);
    }
}
