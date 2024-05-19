using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UguiCardView : CardView
{
    public Image Image;
    public GameObject Indicator;
    private RectTransform rectTransform;
    public Sprite oriSprite;
    public override void Init(CardData cardData, bool sortX)
    {
        if (cardData == null)
        {
            Reset();
        }
        else
            Image.sprite = cardData.Image;
        rectTransform = Image.GetComponent<RectTransform>();
    }

    public override void OnSelect()
    {
        //選擇卡片時，將卡片rectTransform往上移動
        if (rectTransform)
        {
            rectTransform.anchoredPosition += new Vector2(0, OnSelectOffset);
            rectTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            Indicator.SetActive(true);
        }
    }

    public override void OnDeselect()
    {
        //取消選擇卡片時，將卡片rectTransform回到原點
        if (rectTransform)
        {
            rectTransform.anchoredPosition = new Vector2(0, 0);
            rectTransform.localScale = new Vector3(1, 1, 1);
        }
        Indicator.SetActive(false);
    }

    public override void Reset()
    {
        Image.sprite = oriSprite;
        Indicator.SetActive(false);
    }
}
