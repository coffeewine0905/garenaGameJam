using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardView : MonoBehaviour
{
    public float OnSelectOffset = 0.1f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(CardData cardData, bool sortX)
    {
        // this.cardData = cardData;
        if (sortX)
        {
            //往左旋轉25度
            transform.localEulerAngles = new Vector3(-6, -13, 0);
        }
        else
        {
            //往右旋轉25度
            transform.localEulerAngles = new Vector3(-6, 13, 0);
        }
    }

    public void OnSelect()
    {
        //選擇卡片時，將卡片往上移動
        transform.localPosition += new Vector3(0, OnSelectOffset, 0);
    }

    public void OnDeselect()
    {
        //取消選擇卡片時，將卡片回到原點
        transform.localPosition = new Vector3(0, 0, 0);
    }
}
